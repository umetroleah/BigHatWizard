using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private Transform spawnPointFinal;
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;
    public Transform spawnPoint4;
    public Transform spawnPoint5;
    public Transform spawnPoint6;
    public static int spawnPointID = 0;

    private Transform tempPoint;
    public GameObject playerPrefab;
    public CinemachineVirtualCamera vcam;

    public GameObject defualtPrefab;
    public GameObject firePrefab;
    public GameObject icePrefab;

    public static int currentPrefab = 0;




    public void Awake()
    {
        instance = this;

        spawnPointID = SceneLoader.spawnPoint;
        switch (spawnPointID)
        {
            case 1:
                spawnPointFinal = spawnPoint1;
                break;
            case 2:
                spawnPointFinal = spawnPoint2;
                break;
            case 3:
                spawnPointFinal = spawnPoint3;
                break;
            case 4:
                spawnPointFinal = spawnPoint4;
                break;
            case 5:
                spawnPointFinal = spawnPoint5;
                break;
            case 6:
                spawnPointFinal = spawnPoint6;
                break;
            default:
                spawnPointFinal = spawnPoint1;
                break;
        }


        SwapPrefab(currentPrefab);
        MovePlayerToSpawn();
    }

    public void MovePlayerToSpawn()
    {
        PlayerCombat.instance.transform.position = spawnPointFinal.position;
    }

    public void Respawn()
    {
        GameObject gameObject = Instantiate(playerPrefab, spawnPointFinal.position, Quaternion.identity);
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
        int health = PlayerCombat.health;

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

        //Set health to same as before prefab swap
        PlayerCombat.health = health;
        newPlayer.GetComponent<PlayerCombat>().LowerHealthNoHit(health);
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
