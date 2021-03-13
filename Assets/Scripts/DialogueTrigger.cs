using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public Question question;
    public GameObject InteragirText;
    private bool start;


    public void TriggerDialogue()
    {
        if (dialogue.isQuestion == false)
        {
            //Debug.Log("Dialogue simple");
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        }
        if (dialogue.isQuestion == true)
        {
            //Debug.Log("Y'a une question");
            FindObjectOfType<DialogueManager>().StartQuestion(dialogue,question);
            InteragirText.SetActive(false);
        }
        
    }
    private void Start()
    {
        start = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && start)
        {
            TriggerDialogue();
            start = false;
        }
        if(InteragirText == null){
            InteragirText = GameObject.Find("interagirText");
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (dialogue.isQuestion == true)
            {
                if (InteragirText != null)
                {
                    InteragirText.SetActive(true);
                    start = true;
                }
                else
                {
                    Debug.LogWarning("Pas de text pour interagir");
                }
                
            }
            else
            {
                TriggerDialogue();
            }
        }
    }

    //When the Primitive exits the collision, it will change Color
    private void OnTriggerExit(Collider other)
    {
        if(InteragirText != null)
        {
            InteragirText.SetActive(false);
        }
        if (dialogue.isQuestion == false)
        {
            Destroy(this.gameObject);
        }
    }

    }
