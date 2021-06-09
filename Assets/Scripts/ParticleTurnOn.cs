using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTurnOn : MonoBehaviour
{
    ParticleSystem ps;
    bool closeTo;
    public ParticleSystem.EmissionModule em;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        em = ps.emission;

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            em.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            em.enabled = false;
        }
    }
 }
