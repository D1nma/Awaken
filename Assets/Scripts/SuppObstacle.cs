using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuppObstacle : MonoBehaviour
{
    public GameObject obstacle;//à supprimer une fois grimper
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        obstacle.gameObject.SetActive(false);
    }
}
