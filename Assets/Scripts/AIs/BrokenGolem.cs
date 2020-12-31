using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenGolem : MonoBehaviour
{

    GameObject player;
    float lastAttack = 0f;
    private bool attacking = false;
    public float attackCooldown = 2f;

    [SerializeField] private LayerMask m_GroundLayer;
    public float speed;
    public float distance;
    private bool movingRight = true;

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

    public Transform firePointHigh;
    public Transform firePointLow;

    public GameObject bulletPrefab;
    private Rigidbody2D m_rigidbody;

    private MusicManager bgmManager;
    public AudioClip bossMusic;


    //Animation states
    const string IDLE = "BrokenGolem_Idle";
    const string MOVE = "BrokenGolem_Move";
    const string MELEE = "BrokenGolem_Attack1";
    const string RANGED = "BrokenGolem_Attack2";
    const string LOW = "BrokenGolem_Attack3";

    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();

        m_rigidbody = GetComponent<Rigidbody2D>();
        GetComponent<Enemy>().healthBar.SetActive(false);

        bgmManager = FindObjectOfType<MusicManager>();
    }

    private void FindPlayer()
    {
        try
        {
            player = GameObject.FindGameObjectsWithTag("Player")[0];
        }
        catch (MissingReferenceException e)
        {
            player = null;
        }
    }

    void FixedUpdate()
    {
        FindPlayer();

        var direction = Vector2.zero;
        if (player != null)
        {
            //Activate the fight when the player gets near
            if ((player.transform.position - this.transform.position).sqrMagnitude < 20 * 20 && !activeFight)
            {
                //print("Activate Broken Golem Fight");
                activeFight = true;
                bgmManager.ChangeBGM(bossMusic);
                GetComponent<Enemy>().healthBar.SetActive(true);
            }


            //Face player
            if (player.transform.position.x < this.transform.position.x + 1.5f)
            {
                if (movingRight)
                    Flip();
            }
            else if (player.transform.position.x + 1.5f > this.transform.position.x)
            {
                if (!movingRight)
                    Flip();
            }

            //what to do when the fight is active
            if (activeFight)
            {
                FightLoop();
            }
            else if (bgmManager == null)
            {
                bgmManager = FindObjectOfType<MusicManager>();
            }
        }
    }


    void FightLoop()
    {
        //Generate random number to determine action
        float rng = Random.value;

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
            if(rng <= 0.35)
            {
                //Small chance to move closer no matter where the player is
                StopMove();
                StartCoroutine(DoMove());
            }
            else if ((player.transform.position - this.transform.position).sqrMagnitude < 6 * 6 && !attackingMelee && !attackingRanged)
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
            if (movingRight)
                m_rigidbody.velocity = new Vector2(speed, 0);
            else
                m_rigidbody.velocity = new Vector2(-speed, 0);
            //transform.Translate(Vector2.right * speed * Time.deltaTime);
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
        //50 50 chance of high or low attack
        float rng = Random.value;
        Transform firePoint;
        if (rng <= 0.5f)
        {
            firePoint = firePointHigh;
            ChangeAnimationState(RANGED);
        }
        else
        {
            firePoint = firePointLow;
            ChangeAnimationState(LOW);
        }

        //Change animation state and variables
        lastAttack = Time.time;
        attackingRanged = true;

        //Create bullet after delay
        yield return new WaitForSeconds(0.583f);
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    IEnumerator DoMove()
    {
        //Change animation state and some variables, if not too close to a wall
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.left, distance, m_GroundLayer);
        //Debug.DrawRay(wallDetection.position, Vector2.left*distance, Color.red, 10f, false);

        if (movingRight)
            wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.right, distance, m_GroundLayer);

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
        //transform.Translate(Vector2.right * 0 * Time.deltaTime);
        m_rigidbody.velocity = new Vector2(0, 0);
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
        //movingRight = !movingRight;
        //transform.Rotate(0f, 180f, 0f);

        if (movingRight == false)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingRight = !movingRight;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            movingRight = !movingRight;
        }
    }
}
