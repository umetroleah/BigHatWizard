using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleTiles : MonoBehaviour
{
    public Tilemap destructibleTilemap;
    public GameObject destructionEffect;
    [SerializeField] private LayerMask tilesLayer;

    void Start()
    {
        destructibleTilemap = GetComponent<Tilemap>();
    }

    /*void OnCollisionEnter2D(Collision2D collision)
    {
        //print("Collider");
        if (collision.gameObject.CompareTag("Shot"))
        {
            Vector3 hitPosition = Vector3.zero;
            foreach(ContactPoint2D hit in collision.contacts)
            {
                hitPosition.x = Mathf.Round(hit.point.x);
                hitPosition.y = Mathf.Round(hit.point.y);

                //print(collision.gameObject.transform.rotation.eulerAngles);
                //detect in shot is moving left/right and up/down to see how to move hitposition
                bool movingRight = (collision.gameObject.transform.rotation.eulerAngles.y == 0f && Mathf.Abs(collision.gameObject.transform.rotation.eulerAngles.z - 180f) != 90f);
                bool movingLeft = (collision.gameObject.transform.rotation.eulerAngles.y == 180f && Mathf.Abs(collision.gameObject.transform.rotation.eulerAngles.z - 180f) != 90f);
                bool movingUp = (collision.gameObject.transform.rotation.eulerAngles.z == 90f || collision.gameObject.transform.rotation.eulerAngles.z == 45f);
                bool movingDown = (collision.gameObject.transform.rotation.eulerAngles.z == 270f || collision.gameObject.transform.rotation.eulerAngles.z == 315f);
                if (movingRight)
                    hitPosition.x += 0.5f;
                if (movingLeft)
                    hitPosition.x -= 0.5f;
                if (movingUp)
                    hitPosition.y += 0.5f;
                if (movingDown)
                    hitPosition.y -= 0.5f;
                //print("right: " + movingRight + "      down: " + movingDown + "      left: " + movingLeft + "      up: " + movingUp);

                print(hitPosition + " = " + destructibleTilemap.WorldToCell(hitPosition));
                //destructibleTilemap.SetTile(destructibleTilemap.WorldToCell(hitPosition), null);
            }
            Instantiate(destructionEffect, transform.position, transform.rotation);
        }
    }*/

    void OnTriggerEnter2D(Collider2D collider)
    {
        //print("Trigger");
        print(collider.gameObject);
        if (collider.gameObject.CompareTag("Shot"))
        {
            Vector3 hitPosition = collider.gameObject.transform.position;
            for(int i = -1; i<=1; i++)
            {
                for(int j = -1; j<=1; j++)
                {
                    RaycastHit2D hit = Physics2D.Raycast(hitPosition, new Vector2((float) i, (float) j), 1.5f, tilesLayer);
                    //Debug.DrawRay(hitPosition, new Vector2((float)i, (float)j) * 1, Color.red, 10f, false);
                    destructibleTilemap.SetTile(destructibleTilemap.WorldToCell(hit.point), null);
                    Instantiate(destructionEffect, transform.position, transform.rotation);
                }
            }

            
            destructibleTilemap.SetTile(destructibleTilemap.WorldToCell(hitPosition), null);
        }
    }

}
