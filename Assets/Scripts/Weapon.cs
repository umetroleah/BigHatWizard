using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePointForward;
    public Transform firePointForwardUp;
    public Transform firePointForwardDown;
    public Transform firePointUp;
    public Transform firePointDown;
    private Rigidbody2D m_Rigidbody2D;

    public CharacterController2D controller;
    //replace with array of bullet prefabs dependent on weapon equipped later
    public GameObject normalShot;
    public GameObject powerShot;
    public GameObject chargedShot;
    public ParticleSystem chargingEffect;


    public float normalCooldown = 0f;
    public float powerCooldown = 0f;
    private float nextNormalShot;
    private float nextPowerShot;
    private float chargeStart = 0f;
    private bool charging = false;
    public float normalShotKnockback = 1f;
    public float powerShotKnockback = 2f;
    private bool finishCharge = false;


    // Update is called once per frame

    void Start()
    {
        nextNormalShot = Time.time;
        nextPowerShot = Time.time;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        chargingEffect.enableEmission = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextNormalShot)
        {
            StartCoroutine(Shoot(normalShot, true, 0f));
            nextNormalShot = Time.time + normalCooldown;
            chargeStart = Time.time;
            charging = true;
        }

        if (Input.GetButtonDown("Fire2") && Time.time >= nextPowerShot)
        {
            StartCoroutine(Shoot(powerShot, false, 0f));
            nextPowerShot = Time.time + powerCooldown;
            nextNormalShot = Time.time + normalCooldown;
        }

        if (charging && chargeStart + 0.25f <= Time.time)
        {
            chargingEffect.enableEmission = true;
            CinemachineShake.Instance.ShakeCamera(20f, Mathf.Clamp((Time.time - chargeStart) / 2.5f, 0.1f, 0.4f), 0.4f);
            if (chargeStart + 1f <= Time.time && !finishCharge)
            {
                finishCharge = true;
                StartCoroutine(FlashColor(Color.blue));
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if(finishCharge)
            {
                StartCoroutine(Shoot(chargedShot, true, 0f));
            }
            charging = false;
            finishCharge = false;
            chargingEffect.enableEmission = false;
        }


    }

    IEnumerator FlashColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    IEnumerator Shoot(GameObject shot, bool normal, float delay)
    {
        float direction = Input.GetAxisRaw("Vertical");
        bool angle = Input.GetButton("Angle");
        bool grounded = controller.GroundCheck();

        float shotKnockback = 0f;
        if (normal)
        {
            shotKnockback = normalShotKnockback;
        }
        else
        {
            shotKnockback = powerShotKnockback;
            CinemachineShake.Instance.ShakeCamera(20f, 0.5f, 0.2f);
        }

        yield return new WaitForSeconds(delay);
        if (direction > 0)
        {
            if (angle)
            {
                //Shooting up angle
                Instantiate(shot, firePointForwardUp.position, firePointForwardUp.rotation);
                m_Rigidbody2D.AddForce(new Vector2(-2, -2) * shotKnockback * m_Rigidbody2D.mass, ForceMode2D.Impulse);
            }
            else
            {
                //Shooting up
                Instantiate(shot, firePointUp.position, firePointUp.rotation);
                m_Rigidbody2D.AddForce(new Vector2(0, -4) * shotKnockback/1.5f * m_Rigidbody2D.mass, ForceMode2D.Impulse);
            }
        }
        else if (direction < 0 && !grounded)
        {
            if (angle)
            {
                //Shooting down angle
                Instantiate(shot, firePointForwardDown.position, firePointForwardDown.rotation);
                m_Rigidbody2D.AddForce(new Vector2(2, 2) * shotKnockback * m_Rigidbody2D.mass, ForceMode2D.Impulse);
            }
            else
            {
                //Shooting down from air
                Instantiate(shot, firePointDown.position, firePointDown.rotation);
                //If moving up very fast, slow or stop knockback to prevent flying
                if (m_Rigidbody2D.velocity.y > 20)
                    shotKnockback /= 2;
                if (m_Rigidbody2D.velocity.y > 30)
                    shotKnockback = 0f;

                //If moving down, stop movement to allow player to get knocked back up a bit
                if(m_Rigidbody2D.velocity.y <= 0)
                {
                    m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0f);
                }
                m_Rigidbody2D.AddForce(new Vector2(0, 4) * shotKnockback * m_Rigidbody2D.mass, ForceMode2D.Impulse);
            }
        }
        else
        {
            //Shooting forward
            Instantiate(shot, firePointForward.position, firePointForward.rotation);
            if (controller.GetDirection())
                m_Rigidbody2D.AddForce(new Vector2(-4, 0) * shotKnockback * m_Rigidbody2D.mass, ForceMode2D.Impulse);
            else
                m_Rigidbody2D.AddForce(new Vector2(4, 0) * shotKnockback * m_Rigidbody2D.mass, ForceMode2D.Impulse);
        }

        SoundManager.PlaySound("shooting");
    }
}
