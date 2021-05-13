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
    public int SafeValeur = 20;
    public int NotSafeValeur =20;
    bool done;
    public float lookRadius = 8f;
    public GameObject Sun;
    public float DiminutionStress = 0.1f;
    public float minRandom = 15f, maxRandom = 25f,LaValeur,speed=0.2f;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        if (!Sun)
        {
            Sun = GameObject.Find("Directional Light");
        }
        GameObject bar = GameObject.Find("StressBar");
        stressBar = bar.GetComponent<StressBar>();
        currentStress = 0;
        stressBar.SetStress(currentStress);
        stressBar.SetMaxStress(maxStress);
        StartCoroutine(AfterInstance());
        NotSafe = true;
        NewValueStress(minRandom,maxRandom);
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
            Raycast();
            if (!player || dead)
            {
                stressBar.gameObject.SetActive(false);
            }
            else
            {
                stressBar.gameObject.SetActive(true);
                stressBar.SetStress(currentStress);
                /*if (Input.GetKeyDown(KeyCode.Space))
                {
                    StressUp(20);
                }*/
                if (currentStress > 0 && currentStress != maxStress && !NotSafe)
                {
                    StressDownAuto();
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
    }

    void Raycast()
    {
        Debug.Log(done);
        Vector3 fromPosition = new Vector3(transform.position.x, transform.position.y+1f, transform.position.z);
        Vector3 toPosition = Sun.transform.position;
        Vector3 direction = toPosition - fromPosition;

        Ray ray = new Ray(fromPosition, direction);
        Debug.DrawRay(ray.origin, ray.direction * 5f, Color.green);

        RaycastHit hitinfo;

        if(Physics.Raycast(fromPosition, direction,out hitinfo, 10f))
        {
            Debug.Log(hitinfo.transform.gameObject);
            if (hitinfo.transform.gameObject.tag != "Player" && hitinfo.transform.gameObject.tag != "MainCamera" && hitinfo.transform.gameObject.tag != "point" && hitinfo.transform.gameObject.tag != "Invisible")
            {
                if (!done)
                {
                    StressUp(NotSafeValeur);
                }
            }
            
        }
        else
        {
            Debug.Log("Rien");
            if (done)
            {
                StressDown(SafeValeur);
            }
            
            done = false;
        }
    }
    IEnumerator Stress()
    {
        yield return new WaitForSeconds(3);
        NewValueStress(minRandom, maxRandom);
    }

    public void NewValueStress(float a,float b)
    {
        LaValeur = Random.Range(a, b);
    }

    public void StressUp(int stress)
    {
        if(maxRandom <= maxStress)
        {
            minRandom += stress;
            maxRandom += stress;
            if (maxRandom >= maxStress)
            {
                maxRandom = maxStress;
                minRandom = maxRandom - 15;
            }
            if (minRandom <= 0)
            {
                minRandom = 0;
            }
            NewValueStress(minRandom, maxRandom);
            stressBar.SetStress(currentStress);
            done = true;
        }
        if (maxRandom >= maxStress)
        {
            maxRandom = maxStress;
        }

    }

    public void StressDownAuto()
    {
        if (currentStress > 0)
        {
            currentStress -= DiminutionStress;
            stressBar.SetStress(currentStress);
        }
        if (currentStress < 0)
        {
            currentStress = 0;
        }
        
    }
    public void StressDown(int stress)
    {
        if (currentStress > 0)
        {
            minRandom -= stress;
            maxRandom -= stress;
            if (minRandom < 0)
            {
                minRandom = 0;
            }
            if(maxRandom < 0 || maxRandom == minRandom)
            {
                maxRandom = minRandom + 20;
            }
            NewValueStress(minRandom, maxRandom);
            stressBar.SetStress(currentStress);
            done = true;
        }
        if (minRandom < 0)
        {
            minRandom = 0;
        }
        if (maxRandom < 0)
        {
            maxRandom = minRandom + 20;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "SafeZone")
        {
            Debug.Log("Safe Zone");
            if (!done)
            {
                StressDown(SafeValeur);
            }   
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
        if (other.tag == "SafeZone")
        {
            Debug.Log("Safe Zone Exit");
            done = false;
            StressUp(NotSafeValeur);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
