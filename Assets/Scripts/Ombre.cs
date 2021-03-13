﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ombre : MonoBehaviour
{

    GameObject player;
    NavMeshAgent enemy;
    public float LifeTime = 3000f;

    private bool immortal = false;

    public bool idleOne = false;
    private bool canHurt = false;

    private bool follow = true;

    public int degat = 10;
    public float MaxDist = 0.1f;
    public float MinDist = 5;
    public float lookRadius = 10f;
    private float time;

    public Transform[] pathPoints;
    int currentDestinationIndex = 0;



    void Start()
    {

        StartCoroutine(AfterInstance());
        enemy = GetComponent<NavMeshAgent>();
        if (pathPoints.Length != 0)
        {
            currentDestinationIndex = (currentDestinationIndex + 1) % pathPoints.Length;
        }

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
    IEnumerator Recommence()
    {
        yield return new WaitForSeconds(1);
        enemy.isStopped =false;
    }

    // Update is called once per frame
    void Update()
    {
        var ray = new Ray(this.transform.position, this.transform.forward);
        Debug.DrawRay(this.transform.position, this.transform.forward * 2, Color.white);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 30f))
        {
            //Debug.Log(hit.transform.gameObject.tag);
            if (hit.transform.gameObject.tag == "Player")
            {
                //Debug.Log("Salut le joueur");
                canHurt = true;
            }
            else
            {
                canHurt = false;
            }
            if (hit.transform.gameObject.tag == "Decors" && hit.transform.gameObject.tag != "Point" && hit.transform.gameObject.tag != "Spawn" && hit.transform.gameObject.tag != "GM")
            {
                float distance = Vector3.Distance(hit.transform.gameObject.transform.position, transform.position);
                if (distance <= 30f)
                {
                    Debug.Log("Je devrais voir ailleur..");
                    follow = false;
                    enemy.isStopped =true;
                    StartCoroutine(Recommence());
                }
            }

        }
        time += Time.deltaTime;

        if (LifeTime > 0 && !immortal) { LifeTime -= time; }
        if (idleOne)
        {
            immortal = true;
            if (pathPoints.Length > 1)
            {
                StartAgent();
            }

            else
            {
                Debug.LogWarning("Y'a pas de point pour patrouiller");

            }
        }
        if (player != null && follow == true)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            //Debug.Log(enemy.stoppingDistance);
            if (distance <= lookRadius)
            {
                //transform.LookAt(player.transform);
                enemy.destination = player.transform.position;
                if (distance <= enemy.stoppingDistance)
                {
                    FaceTarget();
                    Debug.Log("touché");
                    enemy.isStopped = true;
                    StartCoroutine(Recommence());

                }

            }

        }
        if (LifeTime <= 0)
        {
            Destroy(this.gameObject);
        }

    }
    public void StartAgent()
    {
        enemy.isStopped = false;
        enemy.destination = pathPoints[currentDestinationIndex].position;

    }

    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("quelque chose");
        if (collider.tag == "point")
        {
            //Debug.Log("point");
            Suivant();
        }
    }

    public void Suivant()
    {
        //Debug.Log("touche");
        enemy.isStopped = false;
        currentDestinationIndex = currentDestinationIndex + 1;
        if (currentDestinationIndex == pathPoints.Length)
        {
            currentDestinationIndex = 0;
        }
        enemy.destination = pathPoints[currentDestinationIndex].position;
        //Debug.Log("currentDestinationIndex "+currentDestinationIndex);
    }


    void FaceTarget()
    {
        Vector3 direction = (player.transform.position - transform.position); //.normalized
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 50f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
