using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public Rigidbody2D rb;
    public ParticleSystem particleSystem;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(particleSystem, rb.transform.position, rb.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
