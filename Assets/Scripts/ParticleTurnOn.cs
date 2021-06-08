using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTurnOn : MonoBehaviour
{
    ParticleSystem ps;
    bool closeTo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ps = GetComponent<ParticleSystem>();
        var em = ps.emission;
        if (closeTo)
        {
            em.enabled = true;
        }
        else
        {
            em.enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            closeTo = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            closeTo = false;
        }
    }
 }
