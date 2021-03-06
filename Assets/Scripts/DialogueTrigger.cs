using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public Question question;
    public int son;
    GameManager gm;
    public GameObject InteragirText;
    public bool Interagir,Avertissement,canMove,NotDestroy;
    private bool start;
    public bool sonOn;


    public void TriggerDialogue()
    {
        if (canMove)
        {
            FindObjectOfType<DialogueManager>().canMove = true;
        }
        else
        {
            FindObjectOfType<DialogueManager>().canMove = false;
        }
        if (dialogue.isQuestion == false)
        {
            //Debug.Log("Dialogue simple");
            FindObjectOfType<DialogueManager>().sonOn = sonOn;
            FindObjectOfType<DialogueManager>().son = son;
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
            
        }
        if (dialogue.isQuestion == true)
        {
            //Debug.Log("Y'a une question");
            FindObjectOfType<DialogueManager>().sonOn = sonOn;
            FindObjectOfType<DialogueManager>().son = son;
            FindObjectOfType<DialogueManager>().StartQuestion(dialogue, question);
            
            if (InteragirText != null)
            {
                InteragirText.SetActive(false);
            }
            
        }

    }
    private void Start()
    {

        start = false;
        if (!gm)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        }
    }
    private void Update()
    {
        if (!gm)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        }
        if (!InteragirText)
        {
            InteragirText = gm.InteragirText;
        }
        if (Input.GetKeyDown(KeyCode.E) && start && Interagir)
        {
            TriggerDialogue();
            start = false;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (dialogue.isQuestion == true)
            {
                if (!Interagir)
                {
                    TriggerDialogue();
                }
                if (InteragirText != null && Interagir)
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
                if (Avertissement && !NotDestroy)
                {
                    Destroy(this.gameObject.GetComponent<DialogueTrigger>());
                }
            }
            
        }
    }

    //When the Primitive exits the collision, it will change Color
    private void OnTriggerExit(Collider other)
    {
        if (InteragirText != null&& Interagir)
        {
            InteragirText.SetActive(false);
        }
        if (dialogue.isQuestion == false && !Avertissement && !NotDestroy)
        {
            Destroy(this.gameObject);
        }
        if (other.tag == "Player")
        { start = false; }
        if (other.tag != "Player") { return; }
    }

}
