using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class ActivateOnCollision : MonoBehaviour
{

    public Component component;
    private Behaviour behaviour;
    public float disableTime = 0f;
    public bool disabledAfterTime;


    void Start()
    {
        behaviour = (Behaviour)component;
    }
    

    void OnCollisionEnter2D(Collision2D collision) {
        behaviour.enabled = true;
    }

    void OnCollisionExit2D(Collision2D collsion)
    {
        if(disabledAfterTime)
            StartCoroutine(DisableBehavoirAfterTime(disableTime));
    }


    IEnumerator DisableBehavoirAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        behaviour.enabled = false;
    }
}
