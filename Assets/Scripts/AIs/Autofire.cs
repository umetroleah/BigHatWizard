using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autofire : MonoBehaviour
{

    GameObject player;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public int detectionDistance = 1;
    public float cooldown = 1f;
    private float lastShot = 0f;

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

    void FixedUpdate()
    {
        FindPlayer();
        if ((player.transform.position - this.transform.position).sqrMagnitude < detectionDistance * detectionDistance)
        {
            if (lastShot + cooldown < Time.time)
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                lastShot = Time.time;
            }
        }
    }

}
