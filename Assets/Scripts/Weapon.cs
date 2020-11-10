using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePointForward;
    public Transform firePointUp;
    public Transform firePointDown;
    private Rigidbody2D m_Rigidbody2D;

    public CharacterController2D controller;
    //replace with array of bullet prefabs dependent on weapon equipped later
    public GameObject bulletPrefab;

    public float cooldown = 0;
    private float nextShotTime;
    public float shotKnockback = 1f;

    // Update is called once per frame

    void Start()
    {
        nextShotTime = Time.time;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextShotTime)
        {
            Shoot();
            nextShotTime = Time.time + cooldown;
        }
    }

    void Shoot()
    {
        float direction = Input.GetAxisRaw("Vertical");
        bool grounded = controller.GroundCheck();

        if (direction > 0)
        {
            //Shooting up
            Instantiate(bulletPrefab, firePointUp.position, firePointUp.rotation);
            m_Rigidbody2D.AddForce(new Vector2(0, -4) * shotKnockback * m_Rigidbody2D.mass, ForceMode2D.Impulse);
        }
        else if (direction < 0 && !grounded)
        {
            //Shooting down from air
            Instantiate(bulletPrefab, firePointDown.position, firePointDown.rotation);
            m_Rigidbody2D.AddForce(new Vector2(0, 4) * shotKnockback * m_Rigidbody2D.mass, ForceMode2D.Impulse);
        }
        else
        {
            //Shooting forward
            Instantiate(bulletPrefab, firePointForward.position, firePointForward.rotation);
            if(controller.GetDirection())
                m_Rigidbody2D.AddForce(new Vector2(-4, 0) * shotKnockback * m_Rigidbody2D.mass, ForceMode2D.Impulse);
            else
                m_Rigidbody2D.AddForce(new Vector2(4, 0) * shotKnockback * m_Rigidbody2D.mass, ForceMode2D.Impulse);
        }

        SoundManager.PlaySound("shooting");
    }
}
