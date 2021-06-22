using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class item : MonoBehaviour
{
    public enum ObjectType { Clés=1, Canne=2, Appat=3}
    

    public ObjectType objectType;
    public Inventaire invent;
    GameManager gm;
    public GameObject DialogueClé, DialogueTrigger,DialogueAppat1,DialogueAide;
    float time;
    public static bool StartTime; //une aide si le joueur trouve pas au bout de 5mins
    public GameObject InteragirText;
    private bool dispo,doOnce;


        void Start()
    {
        /*if(objectType == ObjectType.Clés){
            Debug.Log("C'est une clé!");
        }
        if(objectType == ObjectType.Canne){
            Debug.Log("C'est une canne!");
        }*/
        StartCoroutine(Setup());
    }

    IEnumerator Setup()
    {
        yield return new WaitForSeconds(1);
        if (InteragirText == null)
        {
            InteragirText = gm.InteragirText;
        }
        if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }
    }
    IEnumerator DialogueKey(int nb)
    {
        yield return new WaitForSeconds(nb);
        DialogueClé.SetActive(true);
    }
    IEnumerator Disparition(float nb)
    {
        yield return new WaitForSeconds(nb);
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gm)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        }
        if (!invent)
        {
            StartCoroutine(Setup());
        }
        if (StartTime)
        {
            time += Time.deltaTime;
            if(time >= 300)
            {
                DialogueAide.SetActive(true);
                StartTime = false;
                Destroy(this.gameObject,20f);
            }
        }
        if (InteragirText == null)
        {
            InteragirText = gm.InteragirText;
        }

        if (Input.GetKeyDown(KeyCode.E) && dispo){
            InteragirText.SetActive(false);
            dispo = false;
            if(objectType == ObjectType.Clés){
                Destroy(this.gameObject,20f);
                Debug.Log("C'est une clé!");
                StartCoroutine(DialogueKey(7));
                StartCoroutine(Disparition(12f));
                invent.key =true;
                invent.keyEmpty = false;
            }
            if(objectType == ObjectType.Canne){
                Destroy(this.gameObject,20f);
                StartCoroutine(Disparition(1f));
                Debug.Log("C'est une canne!");
                invent.canne =true;
            }
            if(objectType == ObjectType.Appat){
                Destroy(this.gameObject,350f);
                StartCoroutine(Disparition(1f));
                DialogueAppat1.SetActive(true);
                Debug.Log("C'est un Appat!");
                StartTime = true;
                invent.boite =true;
            }
        }
        
    }
    void OnTriggerEnter(Collider player)
    {

        if (player.tag == "Player")
        {
            if (InteragirText != null)
            {
                
                if (objectType == ObjectType.Clés)
                {
                    DialogueTrigger.SetActive(true);
                    InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour intéragir (" + this.gameObject.name + ")";
                    InteragirText.SetActive(true);
                    dispo = true;
                }else if(objectType == ObjectType.Canne)
                {
                    if (!invent.ApresRocher)
                    {
                        if (!doOnce)
                        {
                            AkSoundEngine.PostEvent("CanneFound", gameObject);
                            doOnce = true;
                        }
                        
                        invent.DialoguePeche.SetActive(true);
                        InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour intéragir (" + this.gameObject.name + ")";
                        InteragirText.SetActive(true);
                        dispo = true;
                    }
                }else if(objectType == ObjectType.Appat && Appat.fait)
                {
                    InteragirText.gameObject.GetComponent<Text>().text = this.gameObject.name;
                    InteragirText.SetActive(true);
                }
                else
                {
                    InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour intéragir (" + this.gameObject.name + ")";
                    InteragirText.SetActive(true);
                    dispo = true;
                }

            }
        }
        if (player.tag != "Player") { return; }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (InteragirText != null)
            {
                InteragirText.SetActive(false);
                dispo = false;
            
            }
        }
        if (other.tag != "Player") { return; }
    }
}
