using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationObject : MonoBehaviour
{

    public Rigidbody2D rigidbody2D;
    public float rotationSpeed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody2D.angularVelocity = rotationSpeed;
    }

    void OnDisable()
    {
        rigidbody2D.angularVelocity = 0f;
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    }

}
