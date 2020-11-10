using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WalkAI : MonoBehaviour
{

    [SerializeField] private LayerMask m_GroundLayer;
    public float speed;
    public float distance;

    private bool movingRight = true;

    public Transform wallDetection;

    private void FixedUpdate()
    {
        //Move to the right
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        
        //Change direction if it hits a wall
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.right, distance, m_GroundLayer);
        if (wallInfo.collider == true)
        {
            if (movingRight == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }
    }
}
