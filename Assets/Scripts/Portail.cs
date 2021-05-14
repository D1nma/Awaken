using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portail : MonoBehaviour
{
    public Inventaire invent;
    public GameObject DialogueSansClé;
    public bool open=false;
    // Start is called before the first frame update
    IEnumerator Setup()
    {
        yield return new WaitForSeconds(1);
        if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }
        DialogueSansClé.SetActive(false);
        open = false;
    }
    void Start()
    {
        if (!invent)
        {
            StartCoroutine(Setup());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }


    }
    void OnTriggerEnter(Collider player)
    { 
        if(player.tag == "Player")
        {
            if (!invent.key)
            {
                DialogueSansClé.SetActive(true);
            }
        }
    }
    }
