using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private bool isPaused;
    public AudioMixer audioMixer;

    void Start()
    {
        if (!isPaused)
            CloseMenu();
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            //print("here");
            if (isPaused)
                CloseMenu();
            else
                OpenMenu();
            isPaused = !isPaused;
        }
    }

    public void OpenMenu()
    {
        Time.timeScale = 0;
        pauseMenuUI.SetActive(true);
    }

    public void CloseMenu()
    {
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX_Volume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music_Volume", volume);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master_Volume", volume);
    }
}
