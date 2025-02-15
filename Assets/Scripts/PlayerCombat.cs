﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat instance;

    public int maxHealth = 500;
    public static int health = 500;
    public GameObject deathEffect;
    public Weapon weapon;
    private Rigidbody2D m_Rigidbody2D;
    public float iTime = 1;
    public float knockback = 7;
    private float lastHit = 0;

    public HealthBar healthBar;


    void Awake()
    {
        //DontDestroyOnLoad(transform.gameObject);

        instance = this;

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        if(healthBar == null)
            healthBar = GameObject.FindGameObjectsWithTag("HealthBar")[0].GetComponent<HealthBar>();
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(health);
    }

    public void LowerHealthNoHit(int newHealth)
    {
        health = newHealth;
        healthBar.SetHealth(health);
    }

    public void TakeDamage(int damage)
    {
        //if it doesn't do damage, do nothing
        if (damage == 0) return;


        //Lower health
        health -= damage;
        healthBar.SetHealth(health);

        //Give iFrames
        lastHit = Time.time;

        //Kill if health reaches 0
        if (health <= 0)
        {
            die();
        }

        //Damage flash effect
        SoundManager.PlaySound("hurt");
        StartCoroutine(FlashColor(Color.red));

        CinemachineShake.Instance.ShakeCamera(15f, 0.6f, 0.3f);
    }

    IEnumerator FlashColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void die()
    {
        GameObject deathObject = Instantiate(deathEffect, transform.position, transform.rotation);
        deathObject.transform.localScale = transform.localScale;
        SoundManager.PlaySound("explosion");

        CinemachineShake.Instance.ShakeCamera(10f, 0.8f, 0.5f);

        Destroy(gameObject);
        LevelManager.instance.Respawn();

        //run respawn script
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        CheckHit(hitInfo);
    }

    void OnTriggerStay2D(Collider2D hitInfo)
    {
        CheckHit(hitInfo);
    }

    void CheckHit(Collider2D hitInfo)
    {
        if (lastHit + iTime < Time.time)
        {
            Enemy enemy = hitInfo.GetComponent<Enemy>();
            Hazard hazard = hitInfo.GetComponent<Hazard>();
            if (enemy != null || hazard != null)
            {
                this.TakeDamage(20);

                //knockback
                if (hitInfo.transform.position.x - this.transform.position.x > 0)
                {
                    m_Rigidbody2D.AddForce(new Vector2(-3, 4) * knockback * m_Rigidbody2D.mass, ForceMode2D.Impulse);
                }
                else
                {
                    m_Rigidbody2D.AddForce(new Vector2(3, 4) * knockback * m_Rigidbody2D.mass, ForceMode2D.Impulse);
                }

            }

        }
    }

}
