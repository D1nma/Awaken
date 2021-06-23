using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perdu : MonoBehaviour
{
    public DialogueTrigger DT;
    public Inventaire invent;
    // Start is called before the first frame update
    void Start()
    {
        if (!DT)
        {
            DT = this.gameObject.GetComponent<DialogueTrigger>();
        }
        if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }
        DT.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!DT)
        {
            DT = this.gameObject.GetComponent<DialogueTrigger>();
        }
        else if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }
        if (PersoInteraction.JoueurPasser)
        {
            this.gameObject.SetActive(false);
        }
    }
    void OnTriggerEnter(Collider player)
    {
        if (player.tag == "Player")
        {
            if (DT.enabled)
            {
                AkSoundEngine.PostEvent("Perdu", gameObject);
            }
        }

    }
    void OnTriggerExit(Collider player)
    {
        if (player.tag == "Player")
        {
            if (!DT.enabled && invent.Keyvolee)
            {
                DT.enabled = true;
            }
        }
    }
}
