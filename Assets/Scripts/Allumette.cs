using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Allumette : MonoBehaviour
{
    public GameObject DialogueAllumette;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(this.gameObject,20);
            DialogueAllumette.SetActive(true);
            this.gameObject.SetActive(false);
        }

    }
}
