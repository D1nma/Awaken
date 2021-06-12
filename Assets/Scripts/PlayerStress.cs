using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStress : MonoBehaviour
{
    public float maxStress = 100;
    public float currentStress;
    GameObject player;
    GameManager gm;
    public GameObject DialogueStress;
    public GameObject DialogueLampeEteint;
    public StressBar stressBar;
    public bool dead = false;
    public bool NotSafe = true,Safe =false;
    public static bool LH= false;
    public int SafeValeur = 20;
    public int NotSafeValeur = 3;
    bool done,fait,dialogue=false,dialogue2 = false;
    public float lookRadius = 8f;
    float time;
    bool useTime;
    public GameObject Sun;
    public float DiminutionStress = 0.1f,AugmentationStress = 0.5f;
    public float minRandom, maxRandom,LaValeur,speed=0.2f;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        if (!Sun)
        {
            Sun = GameObject.Find("Directional Light");
        }
        DialogueStress.SetActive(false);
        GameObject bar = GameObject.Find("StressBar");
        stressBar = bar.GetComponent<StressBar>();
        currentStress = 0;
        stressBar.SetStress(currentStress);
        stressBar.SetMaxStress(maxStress);
        StartCoroutine(AfterInstance());
        if (Safe)
        {
            minRandom = 0f;
            maxRandom = 2f;
        }
        if (NotSafe)
        {
            minRandom = 1f;
            maxRandom = 5f;
        }
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
        if (!Sun)
        {
            Sun = GameObject.Find("Directional Light");
        }
        else if (stressBar)
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
                    if (Safe)
                    {
                        minRandom = 0f;
                        maxRandom = 2f;
                    }
                }
                else
                {
                    useTime = false;
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

                }
                if (GameManager.gameOver)
                {
                    dead = true;
                }
                else
                {
                    dead = false;
                }
                if (NotSafe)
                {
                    /*if (currentStress >= LaValeur)
                    {
                        currentStress -= LaValeur * speed * Time.deltaTime;
                    }*/
                    if (currentStress <= LaValeur)
                    {
                        currentStress += LaValeur * speed * Time.deltaTime;
                    }
                    if(currentStress >= LaValeur && currentStress <= maxStress)
                    {
                        StressUpAuto();
                    }
                    

                }
                if (LH)
                {
                    if (!fait)
                    {
                        AugmentationStress = AugmentationStress / 10;
                        fait = true;
                    }
                    
                }
                else
                {
                    if (fait)
                    {
                        AugmentationStress = AugmentationStress * 4;
                        fait = false;
                    }
                }
                if (useTime)
                {
                    time += Time.deltaTime;
                }
                else
                {
                    time = 0;
                }
                if (currentStress >= 80 && !dialogue)
                {
                    DialogueStress.SetActive(true);
                    dialogue = true;
                }
                else if (currentStress < 80)
                {
                    DialogueStress.SetActive(false);
                    dialogue = false;
                }
                else if(currentStress > 60 && currentStress < 80 && !dialogue2)
                {
                    DialogueLampeEteint.SetActive(true);
                    dialogue2 = true;
                }
                else if(currentStress < 60 || currentStress > 80)
                {
                    DialogueLampeEteint.SetActive(false);
                    dialogue2 = false;
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
        //Debug.Log(done);
        Vector3 fromPosition = new Vector3(transform.position.x, transform.position.y+1f, transform.position.z);
        Vector3 toPosition = Sun.transform.position;
        Vector3 direction = toPosition - fromPosition;

        Ray ray = new Ray(fromPosition, direction);
        Debug.DrawRay(ray.origin, ray.direction * 5f, Color.green);

        RaycastHit hitinfo;

        if(Physics.Raycast(fromPosition, direction,out hitinfo, 10f))
        {
            if (hitinfo.transform.gameObject.tag != "Player" 
                && hitinfo.transform.gameObject.tag != "MainCamera" 
                && hitinfo.transform.gameObject.tag != "point"
                && hitinfo.transform.gameObject.tag != "Spawn"
                && hitinfo.transform.gameObject.tag != "UI"
                && hitinfo.transform.gameObject.tag != "GM"
                && hitinfo.transform.gameObject.tag != "Invisible")
            {
                if (done)
                {
                    AugmentationStress = AugmentationStress * 2;
                    done = false;
                }
               
                /*if (!done)
                {
                    StressUp(NotSafeValeur);
                }*/
            }
        }
        else
        {
            if (!done)
            {
                AugmentationStress = AugmentationStress / 2;
                done = true;
            }
            
            //Debug.Log("Rien");
            /*if (done)
            {
                //StressDown(SafeValeur);

                currentStress -= 20;

            }
            
            done = false;*/
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
                if (minRandom <= 0)
                {
                    minRandom = 0;
                }
            }
            NewValueStress(minRandom, maxRandom);
            stressBar.SetStress(currentStress);
            //done = true;
        }
        else if (maxRandom >= maxStress)
        {
            maxRandom = maxStress;
            minRandom = maxRandom - 15;
        }

    }

    public void StressDownAuto()
    {
        if (currentStress > 0)
        {
            useTime = true;
            currentStress -= DiminutionStress * Time.deltaTime + time / 50;
            stressBar.SetStress(currentStress);
        }
        else if (currentStress < 0)
        {
            useTime = false;
            currentStress = 0;
        }
        
    }
    public void StressUpAuto()
    {
        if (currentStress < maxStress)
        {
            currentStress += AugmentationStress * Time.deltaTime;
            stressBar.SetStress(currentStress);
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
            //done = true;
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
            Safe = true;
            NotSafe = false;
            Debug.Log("Safe Zone");
            /*if (!done)
            {
                StressDown(SafeValeur);
            }   */
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SafeZone")
        {
            Safe = false;
            NotSafe = true;
            Debug.Log("Safe Zone Exit");
            //done = false;
            StressUp(NotSafeValeur);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
