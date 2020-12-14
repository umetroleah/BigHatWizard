using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseEffector : MonoBehaviour
{
    public PlatformEffector2D platformEffector2D;
    public float waitTime = 0f;

    void FixedUpdate()
    {
        if (Input.GetButtonUp("Crouch"))
        {
            waitTime = 0.5f;
        }

        if (Input.GetButton("Crouch"))
        {
            if (waitTime <= 0)
            {
                platformEffector2D.rotationalOffset = 180f;
                waitTime = 0.5f;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
        else
        {
            platformEffector2D.rotationalOffset = 0f;
        }
    }
}
