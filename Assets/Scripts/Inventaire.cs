﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventaire : MonoBehaviour
{
    public GameObject bird;
    public GameObject DialogueClé, DialoguePeche, DialoguePeche2,DialogueAppat;
    private static Inventaire instance;

    [HideInInspector]
    public bool Keyvolee=false, ApresRocher=false, First = false,canne = false, boite = false, keyEmpty = false, key = false, champi = false;


    public GameObject canneUI, boiteUI, keyEmptyUI, keyUI, champiUI;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        //this.gameObject.SetActive(false);
        keyEmptyUI.SetActive(false);
        canneUI.SetActive(false);
        boiteUI.SetActive(false);
        champiUI.SetActive(false);
        keyUI.SetActive(false);
        if (DialogueAppat)
        {
            DialogueAppat.SetActive(false);
        }
        if (DialogueClé)
        {
            DialogueClé.SetActive(false);
        }
        if (DialoguePeche)
        {
            DialoguePeche.SetActive(false);
        }
        if (DialoguePeche2)
        {
            DialoguePeche2.SetActive(false);
        }
        if (!bird)
        {
            bird = GameObject.Find("Bird");
        }
    }

    void Update()
    {
        if (CheatCode.cheat)
        {
            ApresRocher = true;
            First = true;
            canne = true;
            boite = true;
            Keyvolee = true;
            key = true;
            champi = true;

        }
        else if(CheatCode.cheat == false)
        {
            ApresRocher = false;
            First = false;
            canne = false;
            Keyvolee = false;
            boite = false;
            key = false;
            champi = false;
        }
        if (!bird)
        {
            bird = GameObject.Find("Bird");
        }
        if (canne)
        {
            canneUI.SetActive(true);          
        }
        if (boite)
        {
            boiteUI.SetActive(true);

        }
        if (champi)
        {
            champiUI.SetActive(true);
        }
        if (key)
        {
            if (!First)
            {
                if (bird)
                {
                    bird.gameObject.SetActive(true);
                    Bird.vol = true;
                }
                Debug.Log("La clé va se faire prendre!");
                keyUI.SetActive(true);
                PlayersController.canControl = false;
                Keyvolee = true;
                RocherOiseau.rocherDialogue = true;
                StartCoroutine(BirdKey(6));
            }
            else
            {
                keyUI.SetActive(true);
                if (DialogueClé)
                {
                    DialogueClé.SetActive(true);
                }
                
                keyEmpty = false;
            }
            
        }
        else if (keyEmpty && !key)
        {
            keyEmptyUI.SetActive(true);
        }
        if (!canne)
        {
            canneUI.SetActive(false);
        }
        if (!boite)
        {
            boiteUI.SetActive(false);
        }
        if (!champi)
        {
            champiUI.SetActive(false);
        }
        if (!key)
        {
            keyUI.SetActive(false);
        }
        if (!keyEmpty)
        {
            keyEmptyUI.SetActive(false);
        }

    }
    private IEnumerator BirdKey(float duree)
    {
        //animator.SetBool("Bird", true);
        yield return new WaitForSeconds(duree);
        bird.SetActive(false);
        key = false;
        keyUI.SetActive(false);
        keyEmpty = true;
        keyEmptyUI.SetActive(true);
        PlayersController.canControl= true;
        First = true;
    }
}
