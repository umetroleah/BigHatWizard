using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, 1f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_GroundLayer;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundPositionBack;                   // A position marking where to check if the player is grounded in the back.
    [SerializeField] private Transform m_GroundPositionFront;                   // A position marking where to check if the player is grounded in the front.
    [SerializeField] private Transform m_CeilingPosition;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisabledCollider;                // A collider that will be disabled when crouching
    [SerializeField] private Collider2D m_CrouchDisabledTrigger;                // A collider that will be disabled when crouching

    const float k_GroundRadius = .25f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private bool m_Jumping;
    private bool m_Falling;
    private bool m_wasFalling;
    private bool m_wasJumping;
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    public bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    public bool doubleJump = false;
    public bool doubleJumpReady = false;


    public bool onIce = false;
    public float iceControl = 0f;


    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;




    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }



    private void FixedUpdate()
    {
        GroundCheck();
        FallCheck();
        JumpCheck();

        //on landing
        if (((m_wasFalling && !m_Falling) || (m_wasJumping && !m_Jumping && !m_Falling)) && m_Grounded)
        {
            OnLandEvent.Invoke();
            if (doubleJump)
                doubleJumpReady = true;
            SoundManager.PlaySound("landing");
        }

        m_wasJumping = m_Jumping;
        m_wasFalling = m_Falling;

        /*
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundPosition.position, k_GroundRadius, m_GroundLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
        */

        //If the player is launched up too fast, slow them down
        /*if((m_Jumping || m_Falling) && m_Rigidbody2D.velocity.y > 30)
        {
            print(m_Rigidbody2D.velocity.y);
            ChangeDrag(m_Rigidbody2D.velocity.y);
        }
        else if(m_Rigidbody2D.drag>10)
        {
            ChangeDrag(0.1f);
        }*/

    }

    public void ChangeDrag(float drag)
    {
        m_Rigidbody2D.drag = drag;
    }

    public void Dash(float dashSpeed)
    {
        if (m_FacingRight)
        {
            m_Rigidbody2D.velocity = Vector2.right * dashSpeed;
        }
        else
        {
            m_Rigidbody2D.velocity = Vector2.left * dashSpeed;
        }
        
    }

    public void Move(float move, bool crouch, bool jump)
    {
        //print(onIce);
        // If crouching, check to see if the character can stand up
        if (!crouch && m_wasCrouching)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingPosition.position, k_CeilingRadius, m_GroundLayer))
            {
                crouch = true;
            }
        }


        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch && !m_Falling && !m_Jumping)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier if not in the air
                if (!m_Falling && ! m_Jumping)
                    move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisabledCollider != null)
                {
                    m_CrouchDisabledCollider.enabled = false;
                    m_CrouchDisabledTrigger.enabled = false;
                }
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisabledCollider != null)
                {
                    m_CrouchDisabledCollider.enabled = true;
                    m_CrouchDisabledTrigger.enabled = true;
                }

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }


            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);

            // And then smoothing it out and applying it to the character
            if (onIce)
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, (1f - iceControl));
            else
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);




            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }



        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            //Jump(10);
        }
    }

    public static float CalculateJumpForce(float gravityStrength, float jumpHeight)
    {
        return Mathf.Sqrt(2 * gravityStrength * jumpHeight);
    }

    public void Jump(float jumpHeight)
    {
        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
        /*if(m_Rigidbody2D.velocity.y < 0)
        {
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
        }*/
        //m_Rigidbody2D.AddForce(new Vector2(0f, jumpForce));
        float jumpForce = CalculateJumpForce(m_Rigidbody2D.gravityScale, jumpHeight);
        m_Rigidbody2D.AddForce(Vector2.up * jumpForce * m_Rigidbody2D.mass, ForceMode2D.Impulse);
        //m_Grounded = false;
    }

    public void CounterJump(float jumpHeight)
    {
        //Debug.Log("Also here");
        float jumpForce = CalculateJumpForce(m_Rigidbody2D.gravityScale, jumpHeight*20);
        m_Rigidbody2D.AddForce(Vector2.down * jumpForce * m_Rigidbody2D.mass);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //print("collide");
        foreach (ContactPoint2D hitPos in collision.contacts)
        {
            if (hitPos.normal.y <= 0) // check if the wall collided on the sides
            {
                //print("wall");
                m_Grounded = false;
            }
            else if (hitPos.normal.y > 0) // check if its collided on top 
            {
                m_Grounded = true;
                //print("grounded " + collision.gameObject);
            }
            else m_Grounded = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D hitPos in collision.contacts)
        {
            if (hitPos.normal.y <= 0) // check if the wall collided on the sides or from below
            {
                //print("wall");
                m_Grounded = false;
            }
            else if (hitPos.normal.y > 0) // check if its collided from above 
            {
                m_Grounded = true;
                //print("grounded");
            }
            else m_Grounded = false;
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        /*Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;*/

        transform.Rotate(0f, 180f, 0f);
    }

    public bool GroundCheck()
    {
        /*if (!Physics2D.OverlapCircle(m_GroundPositionBack.position, k_GroundRadius, m_GroundLayer) &&
            !Physics2D.OverlapCircle(m_GroundPositionFront.position, k_GroundRadius, m_GroundLayer))
        {
            m_Grounded = false;
        }*/

        RaycastHit2D hitFront = Physics2D.Raycast(m_GroundPositionFront.position, -Vector2.up, k_GroundRadius, m_GroundLayer);
        RaycastHit2D hitBack = Physics2D.Raycast(m_GroundPositionBack.position, -Vector2.up, k_GroundRadius, m_GroundLayer);
        if (hitFront.collider == null && hitBack.collider == null)
        {
            //print("here");
            m_Grounded = false;
            onIce = false;
        }
        else
        {
            //print("grounded");
            m_Grounded = true;

            //if there's an ice component on the collider game object, start ice control
            if (hitFront.collider.gameObject.GetComponent<Ice>() != null)
            {
                onIce = true;
            }
            else
            {
                onIce = false;
            }
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

    public bool GetDirection()
    {
        return m_FacingRight;
    }


}
