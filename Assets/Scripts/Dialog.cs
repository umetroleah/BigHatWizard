using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{

    public TextMeshProUGUI textDisplay;
    public string[] sentences;
    private int index;
    public float typeSpeed = 0.02f;
    public bool dialogOpen;

    void Start()
    {
        if (dialogOpen)
            StartCoroutine(Type());
    }

    IEnumerator Type()
    {
        foreach(char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    public void NextSentence()
    {
        if(index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
        {
            textDisplay.text = "";
        }
    }

    public void FixedUpdate()
    {
        if (Input.GetButtonDown("Interact") && dialogOpen)
        {
            NextSentence();
        }
        else{
            dialogOpen = true;
            StartCoroutine(Type());
        }
    }

}
