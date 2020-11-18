using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenGolemAI : MonoBehaviour
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

    private void Flip()
    {
        movingRight = !movingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        var direction = Vector2.zero;
        if (player != null)
        {
            //Activate the fight when the player gets near
            if ((player.transform.position - this.transform.position).sqrMagnitude < 18 * 18 && !activeFight)
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

            if (activeFight)
            {
                //If the attack cooldown has worn off
                if (lastAttack + attackCooldown < Time.time)
                {
                    //print((player.transform.position - this.transform.position).sqrMagnitude + " " + attackingMelee + " " + attackingRanged);
                    //Choose attack based on player distance
                    if ((player.transform.position - this.transform.position).sqrMagnitude < 7 * 7 && !attackingMelee && !attackingRanged)
                    {
                        stopMove();
                        doMelee();
                    }
                    else if ((player.transform.position - this.transform.position).sqrMagnitude < 15 * 15 && !attackingRanged)
                    {
                        stopMove();
                        doRanged();
                    }
                }
                else
                {
                    if ((player.transform.position - this.transform.position).sqrMagnitude < 25 * 25 && (player.transform.position - this.transform.position).sqrMagnitude > 2 * 2 && !attackingMelee && !attackingRanged)
                    {
                        doMove();
                    }
                    else
                    {
                        if (!attackingMelee && !attackingRanged)
                        {
                            stopMove();
                        }
                    }
                }


                //End attack
                if (attackingMelee && lastAttack + meleeTime < Time.time)
                {
                    attackingMelee = false;
                    //Disable collider when not attacking
                    if (m_MeleeDisabledCollider != null)
                    {
                        m_MeleeDisabledCollider.enabled = false;
                    }
                    stopMove();
                }
                if (attackingRanged && lastAttack + rangedTime < Time.time)
                {
                    attackingRanged = false;
                    stopMove();
                }
            }


        }


    }


    void doMelee()
    {
        lastAttack = Time.time;
        attackingMelee = true;
        //animator.SetBool("Melee", true);
        ChangeAnimationState(MELEE);
        //Enable collider when attacking
        if (m_MeleeDisabledCollider != null)
        {
            m_MeleeDisabledCollider.enabled = true;
        }

    }

    void doRanged()
    {
        lastAttack = Time.time;
        attackingRanged = true;
        //animator.SetBool("Ranged", true);
        ChangeAnimationState(RANGED);
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    void doMove()
    {
        //Don't move if it hits wall
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.right, distance, m_GroundLayer);
        if (wallInfo.collider == false)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            //animator.SetFloat("Speed", speed);
            ChangeAnimationState(MOVE);
        }
        else
        {
            stopMove();
        }

    }

    void stopMove()
    {
        transform.Translate(Vector2.right * 0 * Time.deltaTime);
        //animator.SetFloat("Speed", 0);
        ChangeAnimationState(IDLE);
    }

    void ChangeAnimationState(string newState)
    {
        //stop animator from changing state to current state
        if (newState == currentState) return;

        //Play animation and reassign current state
        animator.Play(newState);
        currentState = newState;
    }

}
