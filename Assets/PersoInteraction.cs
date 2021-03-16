using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersoInteraction : MonoBehaviour
{
    public GameObject InteragirText;
    private bool fait=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   /** private void OnTriggerEnter(Collider other)
    {
        if (player.tag == "Player")
        {
            if (InteragirText != null && !fait)
            {
                InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour intéragir ("+this.gameObject.name+")";
                InteragirText.SetActive(true);             
            }
        }
    } **/
}
