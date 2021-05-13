using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventaire : MonoBehaviour
{
    public GameObject bird;
    private static Inventaire instance;

    [HideInInspector]
    public bool First = false,canne = false, boite = false, keyEmpty = false, key = false, champi = false;


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
        if (!bird)
        {
            Debug.LogWarning("Il est ou l'oiseau?");
        }
    }

    void Update()
    {
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
                }
                Debug.Log("La clé va se faire prendre!");
                keyUI.SetActive(true);
                PlayersController.canControl = false;
                Bird.vol = true;
                StartCoroutine(BirdKey(6));
            }
            else
            {
                keyUI.SetActive(true);
                keyEmpty = false;
            }
            
        }
        if (keyEmpty && !key)
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
        key = false;
        keyUI.SetActive(false);
        keyEmpty = true;
        keyEmptyUI.SetActive(true);
        PlayersController.canControl= true;
        First = true;
    }
}
