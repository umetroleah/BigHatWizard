using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{

    public float speed = 20f;
    public Rigidbody2D rb;
    public int damage = 20;
    public GameObject impactEffect;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
        //SoundManager.PlaySound("shooting");
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        GameObject impact = Instantiate(impactEffect, transform.position, transform.rotation, impactEffect.transform.parent);
        impact.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
        SoundManager.PlaySound("shot explode");
        Destroy(gameObject);
    }

}
