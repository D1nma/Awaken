using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lv2Load : MonoBehaviour
{
    public UIManager ui;
    // Start is called before the first frame update
    void Start()
    {
        if (!ui)
        {
            ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!ui)
        {
            ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        ui.loadLv2();
    }
    }
