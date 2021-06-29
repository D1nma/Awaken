using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    GameObject Dialogue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Dialogue)
        {
            Dialogue = GameObject.Find("Dialogue");
        }else if (Dialogue)
        {
            Dialogue.SetActive(false);
        }
        else
        {
            return;
        }
    }
}
