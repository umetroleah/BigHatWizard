using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAI : MonoBehaviour
{
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
    [SerializeField] public float jumpCooldown = 1f;
    [SerializeField] public float jumpHeight = 0.1f;

    public float distance;
    private bool movingRight = true;
    public Transform wallDetection;



    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {
        GroundCheck();
        FallCheck();
        JumpCheck();
        float speedMult = this.GetComponent<Enemy>().speedMult;

        //Debug.Log(m_Grounded);


        if (((m_wasFalling && !m_Falling) || (m_wasJumping && !m_Jumping && !m_Falling)) && m_Grounded)
        {
            //OnLandEvent.Invoke();
            //SoundManager.PlaySound("landing");
            m_Rigidbody2D.velocity = new Vector2(0, 0);
            landTime = Time.time;
        }

        m_wasJumping = m_Jumping;
        m_wasFalling = m_Falling;

        if (landTime + jumpCooldown < Time.time && m_Grounded)
        {
            //Debug.Log("Jumping");
            Jump(jumpHeight * speedMult);
        }


        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.right, distance, m_GroundLayer);
        if (wallInfo.collider == true)
        {
            if (movingRight == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
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
        if(movingRight)
            m_Rigidbody2D.AddForce(new Vector2(1,3) * jumpForce * m_Rigidbody2D.mass, ForceMode2D.Impulse);
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
}
