using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAI : MonoBehaviour
{
    GameObject player;
    [SerializeField] public float flySpeed = 80;
    [SerializeField] public float randomMoves = 1000;
    float lastMove = 0f;


    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        try
        {
            player = GameObject.FindGameObjectsWithTag("Player")[0];
        }
        catch (MissingReferenceException e)
        {
            player = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        FindPlayer();

        var direction = Vector2.zero;
        if (player != null)
        {
            //If player is nearby
            if ((player.transform.position - this.transform.position).sqrMagnitude < 7 * 7)
            {
                if (Vector2.Distance(this.transform.position, player.transform.position) > 0.5)
                {
                    direction = player.transform.position - this.transform.position;
                    direction = Vector3.ClampMagnitude(direction, 0.5f);
                    transform.Translate(direction * flySpeed * Time.deltaTime);
                }
            }
            else
            {
                if (lastMove + randomMoves < Time.time)
                {
                    //Random little movements
                }
            }
        }
    }
}
