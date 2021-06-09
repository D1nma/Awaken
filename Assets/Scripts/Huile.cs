using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Huile : MonoBehaviour
{
    public GameObject InteragirText;
    private LampeHuile lampeHuile;

    private bool dispo;
    // Start is called before the first frame update
    void Start()
    {
        dispo = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (lampeHuile == null)
        {
            lampeHuile = GameObject.Find("Lampe à huile").GetComponent<LampeHuile>();
        }
        if (Input.GetKeyDown(KeyCode.E) && dispo)
        {
            lampeHuile.nbRecharges += 1;
            Debug.Log("Recharge huile : " + lampeHuile.nbRecharges);
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && lampeHuile.EnMain && !dispo)
        {
            if (InteragirText != null)
            {
                dispo = true;
                InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour prendre la recharge";
                InteragirText.SetActive(true);
            }
        }
        if (other.tag == "Player" && lampeHuile && !dispo)
        {
            if (!lampeHuile.EnMain)
            {
                if (InteragirText != null)
                {
                    InteragirText.gameObject.GetComponent<Text>().text = "Il te faut la lampe à huile !";
                    InteragirText.SetActive(true);
                }
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
        if (other.tag != "Player") { return; }
    }

}
