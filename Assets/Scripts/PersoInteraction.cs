using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersoInteraction : MonoBehaviour
{
    public GameObject InteragirText;
    private bool fait = false, canInteract = false,startTiming = false;
    float time;
    private Transform target;
    public float speed = 1.0f;
    public Inventaire invent;
    public GameObject Tips;
    // Start is called before the first frame update
    void Start()
    {
        fait = false;
        target.transform.position = transform.position;
        if (!Tips)
        {
            Debug.LogWarning("Ajouter les parametres necessaires au fonctionnement dans l'inspecteur..");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }
        if (Input.GetKeyUp(KeyCode.E) && canInteract == true)
        {
            if(invent.canne == true){
                PlayersController.canControl = false;
                fait = true;
                float step =  speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            }else{
                startTiming=true;
                Tips.SetActive(true);
                InteragirText.SetActive(false);
                Tips.gameObject.GetComponent<Text>().text = "Il faut la canne à pêche!";
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
            if(fait){
                canInteract = false;
            }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (InteragirText != null && !fait)
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
