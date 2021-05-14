using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Appat : MonoBehaviour
{
    public GameObject InteragirText;
    public GameObject Tips;
    public Inventaire invent;
    public GameObject boiteAppat;
    float time;
    private bool open = false, canInteract = false, startTiming = false;
    // Start is called before the first frame update

    private static Appat instance;
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
    void Start()
    {
        if (!Tips || !InteragirText || !boiteAppat)
        {
            Debug.LogWarning("Ajouter les parametres necessaires au fonctionnement dans l'inspecteur..");
        }
        if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }
        if (canInteract)
        {
            if (Input.GetKeyDown(KeyCode.E) && !open)
            {
                if (invent.boite == true)
                {
                    invent.boite = false;
                    open = true;
                    startTiming = true;
                    Tips.SetActive(true);
                    InteragirText.SetActive(false);
                    Tips.gameObject.GetComponent<Text>().text = "C'est en place!";
                    Instantiate(boiteAppat, transform.position, transform.rotation);
                }
                else
                {
                    startTiming = true;
                    Tips.SetActive(true);
                    Tips.gameObject.GetComponent<Text>().text = "Je devrais trouver une boite d'appat!";
                }
            }
        }
        if (startTiming)
        {
            time += Time.deltaTime;
        }
        if (time >= 4)
        {
            Tips.SetActive(false);
            time = 0;
            startTiming = false;
        }
    }
    void OnTriggerEnter(Collider player)
    {

        if (player.tag == "Player")
        {
            if (InteragirText != null && !open)
            {
                InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour intéragir";
                InteragirText.SetActive(true);
                canInteract = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (InteragirText != null)
            {
                InteragirText.SetActive(false);
                canInteract = false;
            }
        }
    }
}
