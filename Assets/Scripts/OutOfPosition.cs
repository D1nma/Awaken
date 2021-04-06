using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfPosition : MonoBehaviour
{

    private GameManager gm;
    private UIManager ui;
    private bool show = false;
    public static bool enter=false;
    private float time = 0;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        enter = false;
    }


    void Update()
    {
        if (show)
        {
            ui.Respawn.SetActive(true);
            time += Time.deltaTime;
        }
        if (time >= 3)
        {
            show = false;
            ui.Respawn.SetActive(false);
            time = 0;
            enter = false;
        }
        if (enter)
        {
            PlayersController.canControl = false;
            gm.Replace();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            show = true;
            enter = true;
            /*Destroy(gm.Player);
            gm.SpawnPlayer();*/
        }
        if (other.tag == null) { return; }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            
        }
    }

}
