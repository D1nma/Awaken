using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HeadTarget : MonoBehaviour
{
    public float lookRadius = 2.5f;
    GameObject Object;
    public Rig rig = null;
    MultiAimConstraint MA;
    public float speed=4f;
    float distance;
    private bool instancier;
    GameObject oldOne;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AfterInstance());
    }

    // Update is called once per frame
    void Update()
    {
        
        if (instancier)
        {
            if (distance <= lookRadius)
            {
                if (Object != null)
                {
                    if (Object.gameObject.GetComponent<WatchMe>())
                    {
                        if (Object.gameObject.GetComponent<WatchMe>().watchMe)
                        {
                            //Debug.Log("c'est validé");

                            if (MA.weight <= 0.7f)
                            {
                                MA.weight += 0.1f * Time.deltaTime*speed;
                            }
                            else
                            {
                                MA.weight = 0.8f;
                            }
                            oldOne.transform.position = Object.transform.position; 
                        }
                        else
                        {
                            if (MA.weight >= 0f)
                            {
                                MA.weight -= 0.1f * Time.deltaTime*speed;
                            }
                            else
                            {
                                MA.weight = 0;
                            }
                            
                        }
                    }
                    else
                    {
                        if (MA.weight >= 0f)
                        {
                            MA.weight -= 0.1f * Time.deltaTime*speed;
                        }
                        else
                        {
                            MA.weight = 0;
                        }

                    }
                    /*MA.data.sourceObjects.RemoveAt(0);
                    MA.data.sourceObjects.SetTransform(0, Object.transform); //Ne remplace pas et ne supprime pas..
                    Debug.Log(MA.gameObject.name);*/
                }
                else
                {
                    if (MA.weight >= 0f)
                    {
                        MA.weight -= 0.1f * Time.deltaTime*speed;
                    }
                    else
                    {
                        MA.weight = 0;
                    }
                }
            }
            else
            {
                if (MA.weight >= 0f)
                {
                    MA.weight -= 0.1f * Time.deltaTime*speed;
                }
                else
                {
                    MA.weight = 0;
                }
            }
            

            if (Object != null)
            {
                distance = Vector3.Distance(Object.transform.position, transform.position);
            }
        }
    }
    IEnumerator AfterInstance()
    {
        yield return new WaitForSeconds(1);
        instancier = true;
        rig = this.gameObject.GetComponentInChildren<Rig>();
        MA = rig.GetComponentInChildren<MultiAimConstraint>();
        oldOne = MA.data.sourceObjects.GetTransform(0).gameObject;
        if (!rig)
        {
            Debug.Log("Il est ou le rig? Tag le !");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
        {
            //Debug.Log(other.gameObject.name);
            if (other.gameObject.GetComponent<WatchMe>())
            {
                if (other.gameObject.GetComponent<WatchMe>().watchMe)
                {
                    Object = other.gameObject;
                }
            }
            else
            {
                return;
            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<WatchMe>())
        {
            if(other.gameObject.GetComponent<WatchMe>().watchMe)
            {
                Object = null;
            }
            
        }
        else
        {
            return;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
