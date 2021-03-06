using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Serrure : MonoBehaviour
{
    public GameObject InteragirText;
    public GameObject Tips;
    public Inventaire invent;
    GameManager gm;
    float time;
    private bool open = false, canInteract = false, startTiming = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!gm)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        }
        else if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }
        else if (InteragirText == null)
        {
            InteragirText = gm.InteragirText;
        }
        else if (Tips == null)
        {
            Tips = gm.Tips;
        }
        if (canInteract)
        {
            if (Input.GetKeyDown(KeyCode.E) && !open)
            {
                if (invent.key == true)
                {
                    invent.key = false;
                    open = true;
                    Portail.open = true;
                    startTiming = true;
                    Tips.SetActive(true);
                    InteragirText.SetActive(false);
                    Tips.gameObject.GetComponent<Text>().text = "C'est ouvert!";
                }
                else
                {
                    startTiming = true;
                    Tips.SetActive(true);
                    Tips.gameObject.GetComponent<Text>().text = "Vous avez besoin d'une clé!";
                    invent.keyEmpty = true;
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
                InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour intéragir (" + this.gameObject.name + ")";
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
