using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    public string warning;
    GameManager gm;
    public GameObject warningText;
    public static float oldValue;
    public float fogEndValue = 60f;
    public static bool StartFog;
    private float time = 0;
    private bool show = false;

    void Start()
    {
        warningText.GetComponent<Text>().text = warning.ToString();
        oldValue = RenderSettings.fogEndDistance;
        show = false;
        time = 0;
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
    }


    void Update()
    {
        //Debug.Log(RenderSettings.fogEndDistance);
        if (StartFog)
        {
            gm.OutofP = true;
            if (RenderSettings.fogEndDistance > fogEndValue)
            {
                RenderSettings.fogEndDistance -= 2;
            }
            else
            {
                RenderSettings.fogEndDistance = fogEndValue;
            }
        }
        else
        {
            gm.OutofP = false;
            if (RenderSettings.fogEndDistance < oldValue)
            {
                RenderSettings.fogEndDistance += 2;
            }
            else
            {
                RenderSettings.fogEndDistance = oldValue;
            }
        }
        if (show)
        {
            if (warningText != null)
            {
                warningText.SetActive(true);
            }

            time += Time.deltaTime;
        }
        else
        {
            show = false;
            warningText.SetActive(false);
        }
        if (time >= 3)
        {
            show = false;
            warningText.SetActive(false);
            time = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {           
            StartFog = true;
            show = true;
        }
        if (other.tag != "Player") { return; }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            StartFog = false;
            show = false;
        }
        if (other.tag != "Player") { return; }
    }

}
