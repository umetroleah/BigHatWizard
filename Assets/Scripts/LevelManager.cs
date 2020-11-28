using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Transform respawnPoint;
    public Transform tempPoint;
    public GameObject playerPrefab;
    public CinemachineVirtualCamera vcam;

    public GameObject defualtPrefab;
    public GameObject firePrefab;
    public GameObject icePrefab;

    public void Awake()
    {
        instance = this;
    }

    public void Respawn()
    {
        GameObject gameObject = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        vcam.m_Follow = gameObject.transform;
        vcam.Follow = gameObject.transform;
    }

    public void ChangePrefab(int prefab)
    {
        Destroy(GameObject.FindGameObjectsWithTag("Player")[0]);

        if (prefab == 1)
        {
            playerPrefab = firePrefab;
            Respawn();
        }
        else
        {
            playerPrefab = defualtPrefab;
            Respawn();
        }

    }

}
