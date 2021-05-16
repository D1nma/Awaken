using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portail : MonoBehaviour
{
    public Inventaire invent;
    Animator animator;
    public GameObject DialogueSansClé;
    public static bool open=false;
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
        animator = GetComponent<Animator>();
        if (!invent)
        {
            StartCoroutine(Setup());
        }
        open = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            animator.SetBool("open",true);
        }
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
