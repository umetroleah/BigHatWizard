using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int health = 100;
    public GameObject deathEffect;

    public void TakeDamage(int damage)
    {
        //Lower health
        health -= damage;
        //Debug.Log(health);

        //Kill if health reaches 0
        if (health <= 0)
        {
            die();
        }

        //Damage flash effect
        SoundManager.PlaySound("hurt");
        StartCoroutine(FlashColor());
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
