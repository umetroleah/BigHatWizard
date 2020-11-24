using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokeGolem : MonoBehaviour
{

    GameObject player;
    float lastAttack = 0f;
    private bool attacking = false;
    public float attackCooldown = 2f;

    [SerializeField] private LayerMask m_GroundLayer;
    public float speed;
    public float distance;
    private bool movingRight = false;

    public Transform wallDetection;
    public Animator animator;
    private string currentState;

    public bool activeFight = false;

    [SerializeField] private Collider2D m_MeleeDisabledCollider;
    private bool attackingMelee = false;
    public float meleeTime = 0.5f;
    private bool attackingRanged = false;
    public float rangedTime = 0.25f;

    public float moveTime = 0.75f;
    private float lastMove = 0;
    private bool moving = false;
    private bool moveState = false;

    public Transform firePoint;

    public GameObject bulletPrefab;
    private Rigidbody2D m_rigidbody;

    private MusicManager bgmManager;
    public AudioClip bossMusic;


    //Animation states
    const string IDLE = "BrokenGolem_Idle";
    const string MOVE = "BrokenGolem_Move";
    const string MELEE = "BrokenGolem_Attack1";
    const string RANGED = "BrokenGolem_Attack2";

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            player = GameObject.Find("Player");
        }
        catch (MissingReferenceException e)
        {
            player = null;
        }

        m_rigidbody = GetComponent<Rigidbody2D>();

        bgmManager = FindObjectOfType<MusicManager>();
    }

    void FixedUpdate()
    {
        var direction = Vector2.zero;
        if (player != null)
        {
            //Activate the fight when the player gets near
            if ((player.transform.position - this.transform.position).sqrMagnitude < 20 * 20 && !activeFight)
            {
                //print("Activate Broken Golem Fight");
                activeFight = true;
                bgmManager.ChangeBGM(bossMusic);
            }


            //Face player
            if (player.transform.position.x > this.transform.position.x)
            {
                if (movingRight)
                    Flip();
            }
            else
            {
                if (!movingRight)
                    Flip();
            }

            //what to do when the fight is active
            if (activeFight)
            {
                FightLoop();
            }
        }
    }


    void FightLoop()
    {
        //Check if attacks and movements are done. If so, switch to idle. If attack is done, start attack cooldown
        if (attackingMelee && lastAttack + meleeTime < Time.time)
        {
            attackingMelee = false;
            lastAttack = Time.time;
            //Disable collider when not attacking
            if (m_MeleeDisabledCollider != null)
            {
                m_MeleeDisabledCollider.enabled = false;
            }
            StopMove();
        }
        if (attackingRanged && lastAttack + rangedTime < Time.time)
        {
            attackingRanged = false;
            lastAttack = Time.time;
            StopMove();
        }
        if (moving && lastMove + moveTime < Time.time)
        {
            moving = false;
            moveState = false;
            StopMove();
        }

        attacking = attackingMelee || attackingRanged;

        //If attack cooldowns have run out and last movement has completed
        if (lastAttack + attackCooldown < Time.time && !moveState && !attacking)
        {
            if ((player.transform.position - this.transform.position).sqrMagnitude < 8 * 8 && !attackingMelee && !attackingRanged)
            {
                //If close do melee
                StopMove();
                StartCoroutine(DoMelee());
            }
            else if ((player.transform.position - this.transform.position).sqrMagnitude < 13 * 13 && !attackingRanged)
            {
                //If far do ranged
                StopMove();
                StartCoroutine(DoRanged());
            }
            else
            {
                //If very far move closer
                StopMove();
                StartCoroutine(DoMove());
            }
        }

        if (moving)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }



    IEnumerator DoMelee()
    {
        //Change animation state and variables
        lastAttack = Time.time;
        attackingMelee = true;
        ChangeAnimationState(MELEE);

        //Activate Hitbox after a delay
        yield return new WaitForSeconds(0.583f);
        if (m_MeleeDisabledCollider != null)
        {
            m_MeleeDisabledCollider.enabled = true;
        }
    }

    IEnumerator DoRanged()
    {
        //Change animation state and variables
        lastAttack = Time.time;
        attackingRanged = true;
        ChangeAnimationState(RANGED);

        //Create bullet after delay
        yield return new WaitForSeconds(0.583f);
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    IEnumerator DoMove()
    {
        //Change animation state and some variables, if not too close to a wall
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.right, distance, m_GroundLayer);
        if (wallInfo.collider == false)
        {
            lastMove = Time.time;
            moveState = true;
            ChangeAnimationState(MOVE);

            //Allow movement after delay
            yield return new WaitForSeconds(0.25f);
            moving = true;
        }
        else
        {
            StopMove();
            yield return new WaitForSeconds(0f);
        }

    }

    void StopMove()
    {
        transform.Translate(Vector2.right * 0 * Time.deltaTime);
        moveState = false;
        moving = false;
        attackingRanged = false;
        attackingMelee = false;
        ChangeAnimationState(IDLE);
    }

    void ChangeAnimationState(string newState)
    {
        //stop animator from changing state to current state
        if (newState == currentState && currentState != MOVE) return;

        //Play animation and reassign current state
        animator.Play(newState);
        currentState = newState;
    }

    private void Flip()
    {
        movingRight = !movingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
