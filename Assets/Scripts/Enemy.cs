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
    public float speedMult = 1f;

    public ArrayList effects = new ArrayList();
    //Arraylist of arrays to hold effect. {effect, intensity, countdown, duration}
    // 0 - default effect
    // 1 - burn
    // 2 - slow
    // 3 - freeze

    void Start()
    {
        bgmManager = FindObjectOfType<MusicManager>();
        if(healthBar != null)
        {
            healthBar.SetMaxHealth(health);
        }
    }

    void FixedUpdate()
    {
        ArrayList tempList = new ArrayList();

        foreach(float[] effect in effects)
        {
            //burn effect
            if(effect[0] == 1f)
            {

                //countdown to next damage tick
                effect[2] -= Time.deltaTime;

                //Take damage when countdown runs out and reset countdown
                if (effect[2] <= 0f)
                {
                    this.TakeDamage((int)Math.Round(effect[1]));
                    effect[2] += 1f;
                }

                //Color for burning
                GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.1f, 1f);


                effect[3] -= Time.deltaTime;
                if (effect[3] <= 0)
                {
                    GetComponent<SpriteRenderer>().color = Color.white;
                    tempList.Add(effect);
                }
            }

            //slow effect
            if (effect[0] == 2f)
            {

                //Change speedmult
                speedMult = 1 / effect[1];

                //Color for slowed
                GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.4f, 0.8f, 1f);


                effect[3] -= Time.deltaTime;
                if (effect[3] <= 0)
                {
                    GetComponent<SpriteRenderer>().color = Color.white;
                    speedMult = 1f;
                    tempList.Add(effect);
                }
            }

            //Freeze effect
            if (effect[0] == 3f)
            {

                //Change speedmult
                speedMult = 0f;

                //Color for frozen
                GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.3f, 0.7f, 1f);


                effect[3] -= Time.deltaTime;
                if (effect[3] <= 0)
                {
                    GetComponent<SpriteRenderer>().color = Color.white;
                    speedMult = 1f;
                    tempList.Add(effect);
                }
            }

        }

        foreach(float[] remove in tempList)
        {
            effects.Remove(remove);
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
        Effect effect = hitInfo.GetComponent<Effect>();
        if (shot != null)
        {
            this.TakeDamage(shot.damage);

            if(effect != null)
            {
                effects.Add(new float[] {effect.effectCode, effect.intensity, effect.countdown, effect.duration});
            }
        }
    }
}
