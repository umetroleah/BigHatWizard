using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shroomer : MonoBehaviour
{

    public bool movingRight = true;
    GameObject player;
    public Transform wallDetection;
    [SerializeField] private LayerMask m_GroundLayer;
    public float speed;
    public float distance;
    public Rigidbody2D m_rigidbody2D;

    public bool punch;
    public float punchTime;
    public bool spore;
    public float sporeTime;
    private float lastMove = 0f;


    public Animator animator;
    private string currentState;


    //Animation states
    const string IDLE = "Shroomer_Idle";
    const string WALK = "Shroomer_Walk";
    const string PUNCH = "Shroomer_Punch";
    const string SPORE = "Shroomer_Spore";

    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        float speedMult = this.GetComponent<Enemy>().speedMult;
        FindPlayer();
        m_rigidbody2D.velocity = new Vector2(0f, 0f);

        var direction = Vector2.zero;


        //Ensure at the same z level of player to find distance correctly in 2d
        if (this.transform.position.z != player.transform.position.z)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, player.transform.position.z);
        }


        if (player.transform.position.x < this.transform.position.x)
        {
            if (!movingRight)
                Flip();
        }
        else if (player.transform.position.x > this.transform.position.x)
        {
            if (movingRight)
                Flip();
        }


        //If player is in detection range
        if ((player.transform.position - this.transform.position).sqrMagnitude < 10 * 10)
        {
            //if player is still far away
            if ((player.transform.position - this.transform.position).sqrMagnitude > 1 * 1)
            {

                RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.right, distance, m_GroundLayer);
                if (wallInfo.collider == true)
                {
                    ChangeAnimationState(IDLE);
                }
                else
                {
                    ChangeAnimationState(WALK);
                    if (movingRight)
                        m_rigidbody2D.velocity = new Vector2(-speed * speedMult, 0f);
                    else
                        m_rigidbody2D.velocity = new Vector2(speed * speedMult, 0f);
                }


            }
            //if player is in air
            else if (player.transform.position.y > this.transform.position.y + 1f)
            {
                ChangeAnimationState(SPORE);
            }
            else
            {
                ChangeAnimationState(PUNCH);
            }
        }
        else
        {
            ChangeAnimationState(IDLE);
        }

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

    void ChangeAnimationState(string newState)
    {
        //stop animator from changing state to current state
        if (newState == currentState) return;

        //Don't change state from punch or spore unless cooldown is complete
        if ((currentState == PUNCH && lastMove + punchTime > Time.time) || (currentState == SPORE && lastMove + sporeTime > Time.time)) return;

        //Play animation and reassign current state
        animator.Play(newState);
        currentState = newState;
    }

}
