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

        if (managers.Length > 1)
        {
            if (continueMusic && managers[0].GetComponent<MusicManager>().audioSrc == managers[1].GetComponent<MusicManager>().audioSrc)
            {
                Destroy(managers[1].gameObject);
            }
            else
            {
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
