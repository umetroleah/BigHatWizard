using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;
    public Animator animator;
    private string currentState;

    [SerializeField] public float runSpeed = 80;
    [SerializeField] public float jumpHeight = 70;
    public float attackTime = 0.2f;
    private float lastShot = 0f;

    float horizontalMove = 0f;
    bool jump = false;
    bool startJump = false;
    bool jumpHeld = false;
    bool fall = false;
    bool crouch = false;
    bool dash = false;
    bool shoot = false;
    bool bounce = false;

    float dashStart;
    [SerializeField] public float dashTime;
    [SerializeField] public float dashSpeed;
    [SerializeField] public float dashCooldown;
    bool dashAvailable;
    bool groundSinceDash;
    public GameObject dashTrailPrefab;
    bool addDashPrefab = true;

    float lastStep = 0f;

    public float slowDrag = 5f;
    public float normalDrag = 1f;


    //Animation states
    [TextArea] public string mode = "player";
    string DASH;
    string SHOOT;
    string JUMP;
    string FALL;
    string CROUCH;
    string WALK;
    string IDLE;

    void Start()
    {
        DASH = mode + "_dash";
        SHOOT = mode + "_shoot";
        JUMP = mode + "_jump";
        FALL = mode + "_fall";
        CROUCH = mode + "_crouch";
        WALK = mode + "_walk";
        IDLE = mode + "_idle";
    }

    // Update is called once per frame
    void Update()
    {
        //Set movement speed
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        //animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        bounce = controller.bouncing;

        //Check if you were already jumping
        if (controller.JumpCheck())
        {
            jump = true;
            startJump = false;
        }
        //Stop jumping animation, unless its the beginning of the jump before leaving the ground
        else if (!startJump)
        {
            jump = false;
        }


        //Check if you were already falling
        if (controller.FallCheck())
        {
            fall = true;
        }
        else
        {
            fall = false;
        }


        //If jump button is pushed when not already in the air, jump
        if (Input.GetButtonDown("Jump") && !jump && !fall && controller.GroundCheck())
        {
            jump = true;
            jumpHeld = true;
            startJump = true;
            controller.Jump(jumpHeight);
        }
        //If jump button is pushed while in air and double jump is available
        if (Input.GetButtonDown("Jump") && controller.doubleJumpReady && (jump || fall) && !startJump)
        {
            jump = true;
            jumpHeld = true;
            startJump = true;
            controller.Jump(jumpHeight);
            controller.doubleJumpReady = false;
        }
        //Change variable so you can fall faster when jump isn't held anymore
        if (Input.GetButtonUp("Jump"))
        {
            jumpHeld = false;
        }

        //Check for crouch button
        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        //Check if shooting
        if (Input.GetButtonDown("Fire1") && !shoot)
        {
            shoot = true;
            lastShot = Time.time;
        }
        else if(lastShot+attackTime < Time.time)
        {
            shoot = false;
        }


        //Check if no longer dashing
        if (dash && dashStart+dashTime < Time.time)
        {
            dash = false;
        }
        //Check if you have been on ground since dash ended
        if(!groundSinceDash && !dashAvailable && !dash)
        {
            groundSinceDash = controller.GroundCheck();
        }
        //Check if dash is available
        dashAvailable = groundSinceDash && dashStart + dashTime + dashCooldown <= Time.time;
        //Start dashing if button is pressed, you're not already dashing, and it's become available
        if (Input.GetButtonDown("Dash") && !dash && dashAvailable)
        {
            dash = true;
            dashStart = Time.time;
            groundSinceDash = false;
            //animator.SetBool("Dashing", true);
            //ChangeAnimationState(DASH);
            SoundManager.PlaySound("dashing");
            addDashPrefab = true;
        }


        //Change animation based on variable set above
        if (dash)
            ChangeAnimationState(DASH);
        else if (shoot)
            ChangeAnimationState(SHOOT);
        else if (jump)
            ChangeAnimationState(JUMP);
        else if (fall)
            ChangeAnimationState(FALL);
        else if (crouch)
            ChangeAnimationState(CROUCH);
        else if (Mathf.Abs(horizontalMove)>0)
            ChangeAnimationState(WALK);
        else
            ChangeAnimationState(IDLE);



        //Play walking sound if walking and sound wasn't made recently
        if (!dash && !jump && !fall && Input.GetAxisRaw("Horizontal") != 0 && lastStep+0.3 <= Time.time)
        {
            SoundManager.PlaySound("walking");
            lastStep = Time.time;
        }


        //Apply drag when falling, extra drag if jump was kept held down
        if (jumpHeld && fall)
            controller.ChangeDrag(slowDrag);
        else if (fall)
            controller.ChangeDrag(normalDrag);
        else
            controller.ChangeDrag(0.1f);
    }

    public void OnLanding()
    {
        //Debug.Log("Landed");
        //When landing, stop jumping/falling animations
        animator.SetBool("Jumping", false);
        animator.SetBool("Falling", false);
        SoundManager.PlaySound("landing");
    }


    void FixedUpdate()
    {
        //Debug.Log(jump);
        // Move character

        if (!dash)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);


            if (!jumpHeld && !bounce && jump)
            {
                //Debug.Log("here");
                controller.CounterJump(jumpHeight);
            }
            jump = false;
        }
        else
        {
            controller.Dash(dashSpeed);
            //add in dash prefab if one doesn't already exist
            if (addDashPrefab)
            {
                GameObject dashEffect = Instantiate(dashTrailPrefab, this.transform.position, this.transform.rotation);
                //ensure it's facing the correct direction
                if (this.transform.rotation.y < 0)
                    dashEffect.transform.localScale = new Vector3(-1, 1, 1);
                else
                    dashEffect.transform.localScale = new Vector3(1, 1, 1);

                //dashTrailPrefab.GetComponent<Rigidbody2D>().velocity = Vector2.right * dashSpeed;
                dashEffect.transform.parent = this.transform;
                //CinemachineShake.Instance.ShakeCamera(20f, 0.1f, 0.1f);
                addDashPrefab = false;
            }
        }
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
