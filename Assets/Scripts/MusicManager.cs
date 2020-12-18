using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSrc;
    public bool continueMusic;

    void Awake()
    {

        GameObject[] managers = GameObject.FindGameObjectsWithTag("Music");
        print(managers.Length);

        if (managers.Length > 1)
        {
            if (continueMusic && managers[0].GetComponent<MusicManager>().audioSrc == managers[1].GetComponent<MusicManager>().audioSrc)
            {
                print("It should continue playing");
                Destroy(managers[1].gameObject);
            }
            else
            {
                print("It should change music");
                Destroy(managers[0].gameObject);
            }
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void ChangeBGM(AudioClip music)
    {
        audioSrc.Stop();
        audioSrc.clip = music;
        audioSrc.Play();
    }
}
