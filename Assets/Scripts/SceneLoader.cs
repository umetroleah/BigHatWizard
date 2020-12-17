using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public string scene = "Terrain Tester";
    public int spawnSetter = 0;
    public static int spawnPoint;

    

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        spawnPoint = spawnSetter;
        //If the player enters the trigger hitbox
        if (hitInfo.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(scene);
        }

    }

}
