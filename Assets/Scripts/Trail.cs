using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{

    public float speed = 20f;
    public Rigidbody2D rb;
    public ParticleSystem particleSystem;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
    }


    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Shot shot = hitInfo.GetComponent<Shot>();
        Weapon weapon = hitInfo.GetComponent<Weapon>();
        Trail trail = hitInfo.GetComponent<Trail>();
        if (shot == null && weapon == null && trail == null)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            rb.velocity = transform.right * 0;
        }
    }
}
