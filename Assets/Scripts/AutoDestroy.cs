using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{
    public float delay = 0f;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, delay);
    }
}