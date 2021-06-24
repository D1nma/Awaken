using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticuleTurnOff : MonoBehaviour
{
    ParticleSystem ps;
    public ParticleSystem.EmissionModule em;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        em = ps.emission;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            em.enabled = false;
        }
        if (other.tag != "Player") { return; }

    }
}
