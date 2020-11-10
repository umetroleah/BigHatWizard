using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenGolemAI : MonoBehaviour
{

    GameObject player;
    float lastAttack = 0f;
    private bool attacking = false;
    private float attackCooldown = 2f;

    [SerializeField] private LayerMask m_GroundLayer;
    public float speed;
    public float distance;
    private bool movingRight = false;

    public Transform wallDetection;
    public Animator animator;

    public bool activeFight = false;

    [SerializeField] private Collider2D m_MeleeDisabledCollider;
    private bool attackingMelee = false;
    public float meleeTime = 0.5f;
    private bool attackingRanged = false;
    public float rangedTime = 0.25f;
    public Transform firePoint;

    public GameObject bulletPrefab;
    private Rigidbody2D m_rigidbody;



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
            if ((player.transform.position - this.transform.position).sqrMagnitude < 16 * 16 && !activeFight)
            {
                //print("Activate Broken Golem Fight");
                activeFight = true;
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
                    if ((player.transform.position - this.transform.position).sqrMagnitude < 5 * 5 && !attackingMelee && !attackingRanged)
                    {
                        doMelee();
                    }
                    else if ((player.transform.position - this.transform.position).sqrMagnitude < 14 * 14 && !attackingRanged)
                    {
                        doRanged();
                    }
                }
                else
                {
                    //Don't move if it hits wall
                    RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.right, distance, m_GroundLayer);
                    if (wallInfo.collider == false)
                    {
                        transform.Translate(Vector2.right * speed * Time.deltaTime);
                        animator.SetFloat("Speed", m_rigidbody.velocity.x);
                    }
                }


                //End attack
                if (attackingMelee && lastAttack + meleeTime < Time.time)
                {
                    attackingMelee = false;
                    animator.SetBool("Melee", false);
                    //Disable collider when not attacking
                    if (m_MeleeDisabledCollider != null)
                    {
                        m_MeleeDisabledCollider.enabled = false;
                    }
                }
                if (attackingRanged && lastAttack + rangedTime < Time.time)
                {
                    attackingRanged = false;
                    animator.SetBool("Ranged", false);
                }
            }


        }


    }


    void doMelee()
    {
        //print("Doing Melee");
        lastAttack = Time.time;
        attackingMelee = true;
        animator.SetBool("Melee", true);
        //Enable collider when attacking
        if (m_MeleeDisabledCollider != null)
        {
            m_MeleeDisabledCollider.enabled = true;
        }

    }

    void doRanged()
    {
        //print("Doing Ranged");
        lastAttack = Time.time;
        attackingRanged = true;
        animator.SetBool("Ranged", true);
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
