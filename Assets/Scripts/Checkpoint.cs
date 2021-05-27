using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameManager gm;
    private UIManager ui;

    private bool show= false;
    private float time = 0;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        ui = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(show){
            ui.Checkpoint.SetActive(true);
            time += Time.deltaTime;
        }
        if(time >= 3){
            show = false;
            ui.Checkpoint.SetActive(false);
            time = 0;
        }
    }
    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (!gm)
            {
                gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
            }
            if (!ui)
            {
                ui = GameObject.Find("Canvas").GetComponent<UIManager>();
            }
            if(ui && gm)
            {
                show = true;
                gm.lastCheckPointPos = transform.position;
                Debug.Log("Checkpoint" + gm.lastCheckPointPos);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {


    }
}
