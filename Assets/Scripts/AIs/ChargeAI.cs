using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAI : MonoBehaviour
{

    GameObject player;
    float lastCharge = 0f;
    private bool charging = false;
    private float chargeTime = 2f;


    [SerializeField] private LayerMask m_GroundLayer;
    public float speed;
    public float distance;
    private bool movingRight = true;


    public Transform wallDetection;
    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
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

    private void Flip()
    {
        movingRight = !movingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    void FixedUpdate()
    {
        FindPlayer();

        var direction = Vector2.zero;
        if (player != null)
        {
            //If player is nearby and not already charging
            if (Mathf.Abs(player.transform.position.x - this.transform.position.x) < 14 && Mathf.Abs(player.transform.position.y - this.transform.position.y) < 10 && !charging)
            {
                //Get player direction
                if(player.transform.position.x > this.transform.position.x)
                {
                    if (!movingRight)
                        Flip();
                }
                else
                {
                    if (movingRight)
                        Flip();
                }

                //Start charging towards player
                charging = true;

            }

            if (charging)
            {
                //Movement for charge
                //transform.Translate(Vector2.right * speed * Time.deltaTime);
                if(movingRight)
                    this.GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
                else
                    this.GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;


                animator.SetBool("Charging", true);

                //Stop charge if it hits a wall
                RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.right, distance, m_GroundLayer);
                if (wallInfo.collider == true)
                {
                    charging = false;
                }

                //Stop after certain amount of time
                if(lastCharge+chargeTime < Time.time)
                {
                    charging = false;
                    lastCharge = Time.time;
                    animator.SetBool("Charging", false);
                }
            }


        }
    }
}
