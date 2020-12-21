using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public Rigidbody2D rigidbody2D;

    public float xSpeed;
    public float ySpeed;

    public bool startRight;
    public bool startUp;
    private bool movingRight;
    private bool movingUp;

    //public bool reverseOnCollision;
    public float moveTime;
    private float timeLeft;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        movingRight = startRight;
        movingUp = startUp;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveTime != 0f)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f)
            {
                movingUp = !movingUp;
                movingRight = !movingRight;
                timeLeft = moveTime;
            }
        }


        float trueX;
        float trueY;
        if (movingRight)
            trueX = -xSpeed;
        else trueX = xSpeed;
        if (movingUp)
            trueY = ySpeed;
        else trueY = -ySpeed;

        rigidbody2D.velocity = new Vector2(trueX, trueY);
    }
}
