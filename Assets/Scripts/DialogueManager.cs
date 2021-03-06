using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    string nom1, nom2;
    //public AK.Wwise.Event dialogue0, dialogue1, dialogue2, dialogue3, dialogue4, dialogue5, dialogue6, dialogue7, dialogue8, dialogue9, dialogue10;
    public int son;
    public bool sonOn;
    public Text dialogueText;
    public GameObject[] Boutons;
    public Text nameAlyx;
    public Animator animator;
    private float timer, timerlimit = 2;
    private bool Xquestion = false, Yquestion = false, WaitAnswer = false, canClick = false;
    public bool canMove = false;
    private bool dialogueEnd = false, dialogueAvecQuestionEnd = false, display = false, timerStart = false;
    private Queue<sentence> sentences; //mieux qu'un tableau pour le dialogue (FIFO collection)
    private string questions;
    private Question question1;
    private int taille;

    private static DialogueManager instance;


    void Start()
    {
        sentences = new Queue<sentence>();
        dialogueEnd = false;
        dialogueAvecQuestionEnd = false;
        timer = 0;
        display = false;
        Yquestion = false;
        timerStart = false;
        canClick = false;
        WaitAnswer = false;
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (Xquestion == false && Yquestion == false)
            {
                EndDialogue();
            }
            else if (Xquestion == true)
            {
                EndDialogueForQuestion();
            }
            else
            {
                EndQuestion();
            }
        }
        if (WaitAnswer == false)
        {
            if (Input.GetMouseButtonDown(0) && canClick || Input.GetButtonDown("Fire1") && canClick)
            {
                DisplayNextSentence();
            }
        }
        else if (WaitAnswer && Yquestion)
        {
            if (Input.GetMouseButtonDown(0) && canClick || Input.GetButtonDown("Fire1") && canClick)
            {
                timer = 100;
                WaitAnswer = false;
            }

        }
        if (dialogueEnd == true && Xquestion == true && display == true)
        {
            nameAlyx.gameObject.SetActive(true);
            for (int i = 0; i < taille; i++)
            {
                Boutons[i].SetActive(true);
            }
        }
        if (display == false)
        {
            nameAlyx.gameObject.SetActive(false);
            for (int i = 0; i < taille; i++)
            {
                Boutons[i].SetActive(false);
            }
        }
        if (timer >= timerlimit) { EndQuestion(); timerStart = false; timer = 0; return; }
        if (timerStart)
        {
            timer += Time.deltaTime;
        }

    }
    public void StartDialogue(Dialogue dialogue)
    {
        if (sonOn)
        {
            AkSoundEngine.PostEvent("Dialogue_0" + son, gameObject);
        }
        //Debug.Log("Dialogue_0" + son);
        nom1 = dialogue.name;
        nom2 = dialogue.name2;
        if (!canMove)
        {
            PlayersController.canControl = false;
        }
        else
        {
            PlayersController.canControl = true;
        }
        timer += Time.deltaTime;
        nameAlyx.gameObject.SetActive(false);
        for (int i = 0; i < Boutons.Length; i++)
        {
            Boutons[i].SetActive(false);
        }
        animator.SetBool("IsOpen", true);

        sentences.Clear();
        foreach (sentence sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);

        }
        DisplayNextSentence();



    }
    public void StartQuestion(Dialogue dialogue, Question question)
    {
        Cursor.visible = true;
        question1 = question;
        nameAlyx.gameObject.SetActive(true);
        taille = question.choices.Length;
        questions = question.text;
        PlayersController.canControl = false;
        timer += Time.deltaTime;
        if (dialogue.isQuestion == true)
        {
            Xquestion = true;
            for (int i = 0; i < taille; i++)
            {
                Boutons[i].GetComponentInChildren<Text>().text = question.choices[i].text;
            }
        }

        animator.SetBool("IsOpen", true);
        nameText.text = dialogue.name;
        sentences.Clear();

        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            foreach (sentence sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

        }
        DisplayNextSentence();


    }

    public void DisplayNextSentence()
    {
        if (sentences != null)
        {
            canClick = false;
            if (sentences.Count == 0 && Xquestion == false && Yquestion == false)
            {
                EndDialogue();
                return;
            }
            if (sentences.Count == 0 && Xquestion == true)
            {
                EndDialogueForQuestion();
                if (Xquestion && dialogueAvecQuestionEnd && Yquestion == false)
                {
                    display = true;
                    WaitAnswer = true;
                    StartCoroutine(TypeQuestion(questions));
                }
                return;
            }
            sentence sentence = sentences.Dequeue();
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence));

        }

    }

    IEnumerator TypeSentence(sentence sentence)
    {
        nameText.text = sentence.next ? nom2 : nom1;
        dialogueText.text = "";
        foreach (char letter in sentence.sentences.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        canClick = true;
    }
    IEnumerator TypeQuestion(string question)
    {
        dialogueText.text = "";
        foreach (char letter in question.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        canClick = true;
    }

    void EndDialogue()
    {
        //Debug.Log("Fin du dialogue");
        dialogueEnd = true;
        animator.SetBool("IsOpen", false);
        PlayersController.canControl = true;
    }
    void EndDialogueForQuestion()
    {
        //Debug.Log("Fin du dialogue avec question en attente");
        dialogueEnd = true;
        dialogueAvecQuestionEnd = true;
        //animator.SetBool("IsOpen", false);
    }
    void EndQuestion()
    {
        //Debug.Log("Fin des questions");
        animator.SetBool("IsOpen", false);
        StopAllCoroutines();
        Xquestion = false;
        Yquestion = false;
        timerStart = false;
        display = false;
        WaitAnswer = false;
        PlayersController.canControl = true;
        Cursor.visible = false;
        return;
    }

    public void ButtonClicked(int buttonNo)
    {

        //Debug.Log("Button clicked = " + buttonNo);
        if (Yquestion == false) { StartDialogue(question1.choices[buttonNo].dialogue); Yquestion = true; timerStart = true; }
    }
}
