using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStress : MonoBehaviour
{
    public float maxStress = 60;
    public float currentStress;
    GameObject player;

    public StressBar stressBar;
    private float timeStress;
    public bool dead = false;
    public float lookRadius = 8f;

    void Start()
    {
        GameObject bar = GameObject.Find("StressBar");
        stressBar = bar.GetComponent<StressBar>();
        currentStress = 0;
        stressBar.SetStress(currentStress);
        stressBar.SetMaxStress(maxStress);
        StartCoroutine(AfterInstance());
    }

    IEnumerator AfterInstance()
    {
        yield return new WaitForSeconds(1);
        player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
        {
            Debug.Log("Il est ou le joueur? Tag le !");
        }
    }
    void Update()
    {
        if (!player)
        {
            stressBar.gameObject.SetActive(false);
        }
        else
        {
            stressBar.gameObject.SetActive(true);
        }

        /*if (currentStress <= maxStress && !dead)
        {
            StressUp(20);
            timeStress = 0;
        }*/

        timeStress += Time.deltaTime * 0.01f;
        if (currentStress > 0 && currentStress != maxStress)
        {
            StressDown(timeStress);
        }
        if (currentStress < 0)
        {
            currentStress = 0;
            stressBar.SetStress(currentStress);
        }
        if (currentStress >= maxStress)
        {
            currentStress = 0;
            //stressBar.SetStress(currentStress); //annule la réactualisation 
            GameManager.gameOver = true;
            dead = true;

        }



    }

    void StressUp(int stress)
    {
        currentStress += stress;
        stressBar.SetStress(currentStress);
    }

    void StressDown(float stress)
    {
        currentStress -= stress;
        stressBar.SetStress(currentStress);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
