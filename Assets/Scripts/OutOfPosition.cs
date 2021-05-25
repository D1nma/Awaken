using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfPosition : MonoBehaviour
{

    private GameManager gm;
    public GameObject player;
    public CharacterController cc;
    private UIManager ui;
    private bool show = false;
    public static bool enter=false;
    private float time = 0;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        Setup();
        enter = false;
        show = false;
        time = 0;
    }

    IEnumerator Setup()
    {
        yield return new WaitForSeconds(5);
        player = GameObject.FindGameObjectWithTag("Player");
        cc = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
    }


    void Update()
    {
        if (show)
        {
            ui.Respawn.SetActive(true);
            time += Time.deltaTime;
        }
        else
        {
            show = false;
            ui.Respawn.SetActive(false);
            time = 0;
        }
        if (enter)
        {
            PlayersController.canControl = false;
            gm.Replace();
        }
        if (time >= 3)
        {
            show = false;
            ui.Respawn.SetActive(false);
            time = 0;
            enter = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            show = true;
            enter = true;
            if (!player)
            {
                player = other.gameObject;
                cc = player.GetComponent<CharacterController>();
            }
            
            /*Destroy(gm.Player);
            gm.SpawnPlayer();*/
        }
        if (other.tag != "Player") { return; }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            show = false;
        }
        if (other.tag != "Player") { return; }
    }

}
