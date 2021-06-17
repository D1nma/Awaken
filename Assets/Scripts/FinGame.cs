using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinGame : MonoBehaviour
{
    public UIManager ui;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!ui)
        {
            ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        }
    }
    void OnTriggerEnter(Collider player)
    {
        if(player.tag == "Player")
        {
            ui.Fin();
        }
        
    }
}
