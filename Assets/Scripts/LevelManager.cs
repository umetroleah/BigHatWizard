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

    public int currentPrefab;

    public void Awake()
    {
        instance = this;
        currentPrefab = 0;
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
        }else if (prefab == 2)
        {
            playerPrefab = icePrefab;
        }
        else
        {
            playerPrefab = defualtPrefab;
        }

        currentPrefab = prefab;
        Respawn();
    }

    public void SwapPrefab(int prefab)
    {
        GameObject currentPlayer = GameObject.FindGameObjectsWithTag("Player")[0];
        Transform playerPosition = currentPlayer.transform;

        if (prefab == 1)
        {
            playerPrefab = firePrefab;
        }
        else if (prefab == 2)
        {
            playerPrefab = icePrefab;
        }
        else
        {
            playerPrefab = defualtPrefab;
        }

        currentPrefab = prefab;
        GameObject newPlayer = Instantiate(playerPrefab, playerPosition.position, playerPosition.rotation);
        vcam.m_Follow = newPlayer.transform;
        vcam.Follow = newPlayer.transform;

        newPlayer.GetComponent<CharacterController2D>().m_FacingRight = currentPlayer.GetComponent<CharacterController2D>().m_FacingRight;
        Destroy(currentPlayer);
    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown("ModeUp"))
        {
            SwapPrefab((currentPrefab + 1) % 3);
        }
        if (Input.GetButtonDown("ModeDown"))
        {
            SwapPrefab((currentPrefab + 2) % 3);
        }
    }

}
