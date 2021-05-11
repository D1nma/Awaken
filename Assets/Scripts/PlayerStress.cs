using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStress : MonoBehaviour
{
    public float maxStress = 100;
    public float currentStress;
    GameObject player;
    GameManager gm;

    public StressBar stressBar;
    public bool dead = false;
    public bool NotSafe = true;
    public float lookRadius = 8f;
    public float DiminutionStress = 0.1f;
    public float minRandom = 15f, maxRandom = 25f,LaValeur,speed=0.5f;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        GameObject bar = GameObject.Find("StressBar");
        stressBar = bar.GetComponent<StressBar>();
        currentStress = 0;
        stressBar.SetStress(currentStress);
        stressBar.SetMaxStress(maxStress);
        StartCoroutine(AfterInstance());
        NotSafe = true;
        NewValue();
        dead = false;
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
        if (stressBar)
        {
            if (!player || dead)
            {
                stressBar.gameObject.SetActive(false);
            }
            else
            {
                stressBar.gameObject.SetActive(true);
                stressBar.SetStress(currentStress);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StressUp(20);
                }
                if (currentStress > 0 && currentStress != maxStress && !NotSafe)
                {
                    StressDown();
                }
                if (currentStress < 0)
                {
                    currentStress = 0;
                }
                if (currentStress >= maxStress)
                {
                    stressBar.gameObject.SetActive(false);
                    currentStress = 0;
                    //stressBar.SetStress(currentStress); //annule la réactualisation 
                    GameManager.gameOver = true;
                    dead = true;

                }
                if (NotSafe)
                {
                    if (currentStress >= LaValeur)
                    {
                        currentStress -= LaValeur * speed * Time.deltaTime;
                    }
                    if (currentStress <= LaValeur)
                    {
                        currentStress += LaValeur * speed * Time.deltaTime;
                    }

                }
            }
        }
        else
        {
            GameObject bar = GameObject.Find("StressBar");
            stressBar = bar.GetComponent<StressBar>();
        }
        if(GameManager.gameOver == false)
        {
            dead = false;
        }
        

        /*if (currentStress <= maxStress && !dead)
        {
            StressUp(20);
            timeStress = 0;
        }*/
        
    }

    void NewValue()
    {
        LaValeur = Random.Range(minRandom, maxRandom);
    }

    void StressUp(int stress)
    {
        currentStress += stress;
        stressBar.SetStress(currentStress);
    }

    void StressDown()
    {
        currentStress -= DiminutionStress;
        stressBar.SetStress(currentStress);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
