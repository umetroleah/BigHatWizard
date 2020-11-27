using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Transform respawnPoint;
    public GameObject playerPrefab;
    public CinemachineVirtualCamera vcam;

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
}
