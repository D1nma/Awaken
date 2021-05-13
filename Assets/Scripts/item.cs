using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class item : MonoBehaviour
{
    public enum ObjectType { Clés=1, Canne=2, Appat=3, Champi=4 }
    

    public ObjectType objectType;
    public Inventaire invent;

    public GameObject InteragirText;
    private bool dispo;

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
            InteragirText = GameObject.Find("InteragirText");

        }
        if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.E) && dispo){
            InteragirText.SetActive(false);
            dispo = false;
            if(objectType == ObjectType.Clés){
                Destroy(this.gameObject,6.5f);
                Debug.Log("C'est une clé!");
                invent.key =true;
                invent.keyEmpty = false;
            }
            if(objectType == ObjectType.Canne){
                Destroy(this.gameObject);
                Debug.Log("C'est une canne!");
                invent.canne =true;
            }
            if(objectType == ObjectType.Appat){
                Destroy(this.gameObject);
                Debug.Log("C'est un Appat!");
                invent.boite =true;
            }
            if(objectType == ObjectType.Champi){
                Destroy(this.gameObject);
                Debug.Log("C'est un champi!");
                invent.champi =true;
            }
        }
        
    }
    void OnTriggerEnter(Collider player)
    {

        if (player.tag == "Player")
        {
            if (InteragirText != null)
            {
                InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour intéragir ("+this.gameObject.name+")";
                InteragirText.SetActive(true);
                dispo = true;
             
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
                dispo = false;
            
            }
        }
    }
}
