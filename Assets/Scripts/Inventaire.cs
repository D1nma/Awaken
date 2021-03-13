using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventaire : MonoBehaviour
{
    private static Inventaire instance;
    private bool activated = false;

    [HideInInspector]
    public bool canne = false, boite = false, keyEmpty = false, key = false, champi = false;


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
            keyUI.SetActive(true);
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
}
