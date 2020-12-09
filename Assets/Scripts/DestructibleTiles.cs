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

    

    void OnTriggerEnter2D(Collider2D collider)
    {
        //print("Trigger");
        //print(collider.gameObject);
        if (collider.gameObject.CompareTag("Shot"))
        {
            Vector3 hitPosition = collider.gameObject.transform.position;
            for(int i = -1; i<=1; i++)
            {
                for(int j = -1; j<=1; j++)
                {
                    Vector2 direction = new Vector2((float)i, (float)j);
                    direction.Normalize();

                    RaycastHit2D hit = Physics2D.Raycast(hitPosition, direction, 2f, tilesLayer);
                    //Debug.DrawRay(hitPosition, direction * 2, Color.red, 10f, false);

                    Vector2 destructionPoint2 = new Vector2(hit.point.x - 0.1f, hit.point.y);

                    destructibleTilemap.SetTile(destructibleTilemap.WorldToCell(hit.point), null);
                    destructibleTilemap.SetTile(destructibleTilemap.WorldToCell(destructionPoint2), null);
                    Instantiate(destructionEffect, transform.position, transform.rotation);
                }
            }

            
            destructibleTilemap.SetTile(destructibleTilemap.WorldToCell(hitPosition), null);
        }
    }

}
