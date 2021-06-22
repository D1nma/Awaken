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
    bool doOnce;

    void Start()
    {
        if (!warningText)
        {
            warningText = gm.TextWarning;
        }
        warningText.GetComponent<Text>().text = warning.ToString();
        oldValue = RenderSettings.fogEndDistance;
        show = false;
        time = 0;
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
    }


    void Update()
    {
        if (!gm)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        }
        else if (!warningText)
        {
            warningText = gm.TextWarning;
        }
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
        else if(!StartFog)
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
                Debug.Log("warning");
            }

            time += Time.deltaTime;
        }
        /*else if(!show)
        {
            show = false;
            if (warningText)
            {
                warningText.SetActive(false);
            }            
        }*/
        if (time >= 3)
        {
            show = false;
            if (warningText)
            {
                warningText.SetActive(false);
            }
            time = 0;
        }
        if (!show)
        {
            warningText.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!doOnce)
            {
                AkSoundEngine.PostEvent("Brume_Start", gameObject);
                doOnce = true;
            }
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
