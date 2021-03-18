using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersoInteraction : MonoBehaviour
{
    public GameObject InteragirText;
    private bool fait = false, canInteract = false;
    private Transform target;
    public float speed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        fait = false;
        target.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E) && canInteract == true)
        {
            PlayersController.canControl = false;
            fait = true;
            float step =  speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);

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
}
