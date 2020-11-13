using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{

    public float speed = 20f;
    public Rigidbody2D rb;
    public ParticleSystem particleSystem;
    public float duration = 0.542f;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
        startTime = Time.time;
    }

    void FixedUpdate()
    {
        if (startTime + duration < Time.time)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            rb.velocity = transform.right * 0;
        }
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
