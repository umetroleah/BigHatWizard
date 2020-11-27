using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    private float length;
    private float startpos;
    public GameObject camera;
    public float parallax;
    public bool scroll = false;
    public float scrollSpeed = -0f;



    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float temp = (camera.transform.position.x * (1 - parallax));
        float dist = (camera.transform.position.x * parallax);
        float newPos = 0f;
        if (scroll)
        {
            newPos = Mathf.Repeat(Time.time * scrollSpeed, length);
        }

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z) + Vector3.right * newPos;

        if (temp > startpos + length)
        {
            startpos += length;
        }
        else if (temp < startpos - length)
        {
            startpos -= length;
        }
    }
}
