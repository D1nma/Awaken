using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Allumette : MonoBehaviour
{
    public GameObject InteragirText;
    public GameObject DialogueAllumette;
    private bool dispo;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(true);
        dispo = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && dispo)
        {

            Destroy(this.gameObject, 20);
            DialogueAllumette.SetActive(true);
            this.gameObject.SetActive(false);
            dispo = false;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            dispo = true;
            InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour prendre la boite à allumette";
            InteragirText.SetActive(true);
        }
        if (other.tag != "Player") { return; }

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
