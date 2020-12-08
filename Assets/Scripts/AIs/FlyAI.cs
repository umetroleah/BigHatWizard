using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAI : MonoBehaviour
{
    GameObject player;
    [SerializeField] public float flySpeed = 80;
    [SerializeField] public float randomMoves = 1000;
    float lastMove = 0f;
    public int followDistance = 7;
    private Rigidbody2D m_rigidbody;
    public bool flipable = false;
    public bool movingRight = true;


    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
        m_rigidbody = GetComponent<Rigidbody2D>();
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

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        float speedMult = this.GetComponent<Enemy>().speedMult;
        FindPlayer();
        m_rigidbody.velocity = new Vector2(0f, 0f);

        var direction = Vector2.zero;
        if (player != null)
        {
            //If player is nearby
            if ((player.transform.position - this.transform.position).sqrMagnitude < followDistance * followDistance)
            {
                if (Vector2.Distance(this.transform.position, player.transform.position) > 0.5)
                {
                    if(movingRight)
                        direction = new Vector3(player.transform.position.x - this.transform.position.x, player.transform.position.y - this.transform.position.y, this.transform.position.z);
                    else
                        direction = new Vector3(this.transform.position.x - player.transform.position.x, player.transform.position.y - this.transform.position.y, this.transform.position.z);
                    direction = Vector3.ClampMagnitude(direction, 0.5f);
                    transform.Translate(direction * flySpeed * speedMult * Time.deltaTime);
                }
            }
            else
            {
                if (lastMove + randomMoves < Time.time)
                {
                    //Random little movements
                }
            }
        }

        
        if ((player.transform.position.x > this.transform.position.x && movingRight) || (player.transform.position.x < this.transform.position.x && !movingRight))
        {
            Flip();
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
}
