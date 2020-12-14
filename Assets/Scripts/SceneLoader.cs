using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public string scene = "Terrain Tester";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        //If the player enters the trigger hitbox
        if (hitInfo.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(scene);
        }

    }

}
