using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int health = 100;
    public GameObject deathEffect;

    private MusicManager bgmManager;
    public AudioClip newMusic;
    public bool changeMusicOnDeath = false;

    public HealthBar healthBar;


    void Start()
    {
        bgmManager = FindObjectOfType<MusicManager>();
        if(healthBar != null)
        {
            healthBar.SetMaxHealth(health);
        }
    }


    public void TakeDamage(int damage)
    {
        //Lower health
        health -= damage;
        if(healthBar != null)
        {
            healthBar.SetHealth(health);
        }

        //Kill if health reaches 0
        if (health <= 0)
        {
            die();
        }

        //Damage flash effect
        SoundManager.PlaySound("hurt");
        StartCoroutine(FlashColor());

        CinemachineShake.Instance.ShakeCamera(10f, 0.20f, 0.1f);

    }
    
    IEnumerator FlashColor()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void die()
    {
        GameObject deathObject = Instantiate(deathEffect, transform.position, transform.rotation);
        deathObject.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
        SoundManager.PlaySound("explosion");

        if (changeMusicOnDeath)
        {
            bgmManager.ChangeBGM(newMusic);
        }

        CinemachineShake.Instance.ShakeCamera(15f, 0.5f, 0.2f);

        if (healthBar != null)
            Destroy(GetComponent<Enemy>().healthBar);

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Shot shot = hitInfo.GetComponent<Shot>();
        if (shot != null)
        {
            this.TakeDamage(shot.damage);
        }
    }
}
