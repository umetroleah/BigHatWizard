﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{

    public float speed = 20f;
    public Rigidbody2D rb;
    public int damage = 20;
    public GameObject impactEffect;
    public GameObject trailPrefab;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
        Instantiate(trailPrefab, rb.transform.position, rb.transform.rotation);
        //SoundManager.PlaySound("shooting");
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Trail trail = hitInfo.GetComponent<Trail>();
        Weapon weapon = hitInfo.GetComponent<Weapon>();
        if (trail == null && weapon == null)
        {
            GameObject impact = Instantiate(impactEffect, transform.position, transform.rotation, impactEffect.transform.parent);
            impact.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
            SoundManager.PlaySound("shot explode");
            Destroy(gameObject);
        }
    }

}
