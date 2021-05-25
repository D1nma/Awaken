using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PontPasser : MonoBehaviour
{
    public GameObject dialogue;
    // Start is called before the first frame update
    void Start()
    {
        dialogue.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            dialogue.SetActive(true);
        }
        if (other.tag != "Player") { return; }
    }
}
