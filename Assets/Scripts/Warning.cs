using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    public string warning;
    public GameObject warningText;

    void Start()
    {
        warningText.GetComponent<Text>().text = warning.ToString();
    }

    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (warningText != null && warning !=null)
                {
                    warningText.SetActive(true);
                }else{
                    Debug.LogWarning("Manque un paramètre");
                }
    }
    private void OnTriggerExit(Collider other)
    {
        if (warningText != null && warning !=null)
                {
                    warningText.SetActive(false);
                }else{
                    Debug.LogWarning("Manque un paramètre");
                }
    }

}
