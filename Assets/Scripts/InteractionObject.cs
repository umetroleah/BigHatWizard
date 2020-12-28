using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionObject : MonoBehaviour
{

    GameObject player;


    public bool canTalk;
    public string message;
    public GameObject textBox;
    public TextMeshProUGUI textDisplay;
    public string[] sentences;
    private int index = 0;
    public float typeSpeed = 0.02f;
    public bool dialogOpen;
    private GameObject opened;

    // Start is called before the first frame update
    void Start()
    {
        textBox.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FindPlayer();



        //if player is nearby and the interact script was pressed
        if (Input.GetButtonDown("Interact") && Mathf.Abs(player.transform.position.x - this.transform.position.x) < 2 && Mathf.Abs(player.transform.position.y - this.transform.position.y) < 2)
        {
            if (canTalk)
            {
                //If dialog box isn't already open, open it and start typing, otherwise go to the next sentence
                if (!dialogOpen)
                {
                    opened = this.gameObject;
                    index = 0;
                    textBox.SetActive(true);
                    dialogOpen = true;
                    StartCoroutine(Type());
                    //textBox.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = message;
                }
                else
                {
                    NextSentence();
                }
            }
        }


        //If player walks away or completes the dialog
        if(opened != null && ( (Mathf.Abs(player.transform.position.x - this.transform.position.x) > 2 || Mathf.Abs(player.transform.position.y - this.transform.position.y) > 2)   || !dialogOpen))
        {
            if (canTalk)
            {
                textDisplay.text = "";
                textBox.SetActive(false);
                dialogOpen = false;
                opened = null;
            }
        }

    }


    private void FindPlayer()
    {
        try
        {
            player = GameObject.FindGameObjectsWithTag("Player")[0];
        }
        catch (MissingReferenceException e)
        {
            player = null;
        }
    }

    IEnumerator Type()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            SoundManager.PlaySound("walking");
            yield return new WaitForSeconds(typeSpeed);

            //Stop typing current message if you try to continue to next line or if dialog box has already closed
            if (Input.GetButtonDown("Interact") || !dialogOpen)
            {
                textDisplay.text = "";
                yield break;
            }
        }
    }

    public void NextSentence()
    {

        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
        {
            dialogOpen = false;
        }
    }



}
