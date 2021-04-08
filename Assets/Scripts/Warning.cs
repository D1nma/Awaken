using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    public string warning;
    public GameObject warningText;
    public static float oldValue;
    public float fogEndValue = 60f;
    public static bool StartFog;

    void Start()
    {
        warningText.GetComponent<Text>().text = warning.ToString();
        oldValue = RenderSettings.fogEndDistance;
    }


    void Update()
    {
        //Debug.Log(RenderSettings.fogEndDistance);
        if (StartFog)
        {
            if(RenderSettings.fogEndDistance > fogEndValue)
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
            if(RenderSettings.fogEndDistance < oldValue)
            {
                RenderSettings.fogEndDistance += 2;
            }
            else
            {
                RenderSettings.fogEndDistance = oldValue;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartFog = true;
            if (warningText != null && warning != null)
            {
                warningText.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Manque un paramètre");
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            StartFog = false;
            if (warningText != null && warning != null)
            {
                warningText.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Manque un paramètre");
            }
        }

    }

}
