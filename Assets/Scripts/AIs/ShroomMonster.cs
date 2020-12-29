using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShroomMonster : MonoBehaviour
{
    public bool activeFight = false;

    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private Transform m_GroundPositionBack;
    [SerializeField] private Transform m_GroundPositionFront;
    private Rigidbody2D m_Rigidbody2D;

    const float k_GroundRadius = .1f;
    private bool m_Grounded;
    private bool m_Jumping;
    private bool m_Falling;
    private bool m_wasFalling;
    private bool m_wasJumping;
    private float landTime = 0f;
    [SerializeField] public float jumpHeight = 0.1f;

    public float distance;
    private bool movingRight = true;
    public Transform wallDetection;
    public GameObject player;


    float lastAttack = 0f;
    private bool attacking = false;
    public float attackCooldown = 2f;
    public float sporeTime = 0f;
    private bool attackingSpore = false;
    public float stompTime = 0f;
    private bool attackingStomp = false;
    private float speedMult;



    public Animator animator;
    private string currentState;

    //Animation states
    const string IDLE = "ShroomBoss_Idle";
    const string JUMP = "ShroomBoss_Jump";
    const string FALL = "ShroomBoss_Fall";
    const string SPORE = "ShroomBoss_Spore";
    const string STOMP = "ShroomBoss_Stomp";


    public GameObject sporePrefab;
    private MusicManager bgmManager;
    public AudioClip bossMusic;

    [SerializeField] private Collider2D m_SporeDisabledCollider;

    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        FindPlayer();
        GetComponent<Enemy>().healthBar.SetActive(false);
        bgmManager = FindObjectOfType<MusicManager>();
    }

    private void FixedUpdate()
    {
        m_wasFalling = m_Falling;
        m_wasJumping = m_Jumping;
        GroundCheck();
        FallCheck();
        JumpCheck();
        FindPlayer();
        speedMult = this.GetComponent<Enemy>().speedMult;

        //Restart attack cooldown when boss hits ground. And stop velocity when landing
        if (m_wasFalling && m_Grounded)
        {
            //OnLandEvent.Invoke();
            //SoundManager.PlaySound("landing");
            StopMove();
            landTime = Time.time;
            lastAttack = landTime;
        }


        //Make sure it always jumps towards player
        if(player.transform.position.x - this.transform.position.x > 0)
        {
            movingRight = true;
        }
        else
        {
            movingRight = false;
        }


        var direction = Vector2.zero;
        if (player != null)
        {
            //Activate the fight when the player gets near
            if (Mathf.Abs(player.transform.position.x - this.transform.position.x) < 25 && Mathf.Abs(player.transform.position.y - this.transform.position.y) < 10 && !activeFight)
            {
                activeFight = true;
                bgmManager.ChangeBGM(bossMusic);
                GetComponent<Enemy>().healthBar.SetActive(true);
            }


            /*//Face player
            if (player.transform.position.x < this.transform.position.x + 1.5f)
            {
                if (movingRight)
                    Flip();
            }
            else if (player.transform.position.x + 1.5f > this.transform.position.x)
            {
                if (!movingRight)
                    Flip();
            }*/

            //what to do when the fight is active
            if (activeFight)
            {
                FightLoop();
            }
        }
    }





    void FightLoop()
    {
        //Generate random number to determine action
        float rng = Random.value;

        //Check if attacks are done
        if (attackingSpore && lastAttack + sporeTime < Time.time)
        {
            attackingSpore = false;
            lastAttack = Time.time;
            //Disable collider when not attacking
            if (m_SporeDisabledCollider != null)
            {
                m_SporeDisabledCollider.enabled = false;
            }
            StopMove();
        }
        if (attackingStomp && lastAttack + sporeTime < Time.time)
        {
            attackingStomp = false;
            lastAttack = Time.time;
            StopMove();
        }



        //If falling, change animation state accordingly
        if (m_Falling)
        {
            ChangeAnimationState(FALL);
        }

        //Only act when boss is on the ground, and cooldown has run out
        if (m_Grounded && lastAttack + attackCooldown < Time.time)
        {
            //If player is close and above shroom monster, do spore attack
            if (Mathf.Abs(player.transform.position.x - this.transform.position.x) < 5 && player.transform.position.y - this.transform.position.y > 1.5)
            {
                StopMove();
                StartCoroutine(DoSpore());
            }

            //If player is too far, do a jump towards player
            else if (Mathf.Abs(player.transform.position.x - this.transform.position.x) > 5)
            {
                StopMove();
                StartCoroutine(DoJump(1f));
            }

            //Do random action if player is a medium distance away
            else if (rng < 0.3)
            {
                StopMove();
                float smallMult = Random.Range(0.4f, 0.6f);
                StartCoroutine(DoJump(smallMult));
            }
            else if(rng < 0.5)
            {
                StopMove();
                StartCoroutine(DoSpore());
            }
            else if(rng<0.9)
            {
                StopMove();
                StartCoroutine(DoStomp());
            }
            else
            {
                StopMove();
            }

        }
    }



    IEnumerator DoSpore()
    {
        lastAttack = Time.time;
        ChangeAnimationState(SPORE);
        attackingSpore = true;

        //Activate spore cloud after a delay
        yield return new WaitForSeconds(0.583f);
        if (m_SporeDisabledCollider != null)
        {
            m_SporeDisabledCollider.enabled = true;
        }
        Instantiate(sporePrefab, transform.position, transform.rotation);
    }

    IEnumerator DoJump(float smallMult)
    {
        lastAttack = Time.time;
        ChangeAnimationState(JUMP);


        yield return new WaitForSeconds(0f);
        float jumpMult = Random.Range(0.8f, 1.1f) * smallMult;
        Jump(jumpHeight * speedMult * jumpMult);
    }

    IEnumerator DoStomp()
    {
        lastAttack = Time.time;
        ChangeAnimationState(STOMP);
        attackingStomp = true;

        yield return new WaitForSeconds(0f);
    }


    void StopMove()
    {
        //transform.Translate(Vector2.right * 0 * Time.deltaTime);
        m_Rigidbody2D.velocity = new Vector2(0, 0);
        ChangeAnimationState(IDLE);
        attackingStomp = false;
        attackingSpore = false;
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



    public static float CalculateJumpForce(float gravityStrength, float jumpHeight)
    {
        return Mathf.Sqrt(2 * gravityStrength * jumpHeight);
    }

    public void Jump(float jumpHeight)
    {
        if (m_Rigidbody2D.velocity.y < 0)
        {
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
        }
        float jumpForce = CalculateJumpForce(m_Rigidbody2D.gravityScale, jumpHeight);
        if (movingRight)
            m_Rigidbody2D.AddForce(new Vector2(1, 3) * jumpForce * m_Rigidbody2D.mass, ForceMode2D.Impulse);
        else
            m_Rigidbody2D.AddForce(new Vector2(-1, 3) * jumpForce * m_Rigidbody2D.mass, ForceMode2D.Impulse);

        //m_Grounded = false;
    }



    public bool GroundCheck()
    {
        RaycastHit2D hitFront = Physics2D.Raycast(m_GroundPositionFront.position, -Vector2.up, k_GroundRadius, m_GroundLayer);
        RaycastHit2D hitBack = Physics2D.Raycast(m_GroundPositionBack.position, -Vector2.up, k_GroundRadius, m_GroundLayer);
        if (hitFront.collider == null && hitBack.collider == null)
        {
            //print("here");
            m_Grounded = false;
        }
        else
        {
            //print("grounded");
            m_Grounded = true;
        }

        return m_Grounded;
    }

    public bool JumpCheck()
    {
        //GroundCheck();
        if (m_Rigidbody2D.velocity.y > 0.001 && !m_Grounded)
        {
            m_Jumping = true;
            //Debug.Log("I am jumping");
        }
        else
        {
            m_Jumping = false;
        }

        return m_Jumping;
    }

    public bool FallCheck()
    {
        //GroundCheck();
        if (m_Rigidbody2D.velocity.y < -0.001 && !m_Grounded)
        {
            m_Falling = true;
            //Debug.Log("I am falling");
        }
        else
        {
            m_Falling = false;
        }

        return m_Falling;
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
