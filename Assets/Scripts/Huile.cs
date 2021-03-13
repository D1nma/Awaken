using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Huile : MonoBehaviour
{
    private LampeHuile lampeHuile;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (lampeHuile == null)
        {
            lampeHuile = GameObject.Find("Lampe à huile").GetComponent<LampeHuile>();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && lampeHuile.EnMain)
        {
            
            lampeHuile.nbRecharges += 1;
            Debug.Log("Recharge huile : " + lampeHuile.nbRecharges);
            Destroy(this.gameObject);
        }

    }

}
