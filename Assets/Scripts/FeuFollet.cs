using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class FeuFollet : MonoBehaviour
{
    public static bool pieger = true,follow, canInteract, done;
    public GameObject player, spotPNJ;
    public GameObject InteragirText;
    NavMeshAgent ff;
    public Transform InitialSpawn;
    //public GameObject TheNPC;
    public float FollowSpeed;
    public RaycastHit shot;
    Vector3 trait;
    public float offsetRay = 0.5f;

    public static FeuFollet instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        ff = GetComponent<NavMeshAgent>();
        done = false;
        follow = false;
        pieger = true;
        ff.isStopped = true;
    }

    void Update()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else if (!spotPNJ)
        {
            spotPNJ = GameObject.Find("SpotPNJ");
        }
        else if (Input.GetKeyUp(KeyCode.E) && canInteract == true)
        {
            InteragirText.SetActive(false);
            done = true;
            pieger = false;
            follow = true;
        }
        if (!pieger && follow && player)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            trait = this.transform.position;
            trait.y = this.transform.position.y + offsetRay;
            FaceTarget();
            
            Debug.DrawRay(trait, transform.TransformDirection(Vector3.forward));
            if (Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out shot))
            {
                //TargetDistance = shot.distance;
                if (distance > ff.stoppingDistance)
                {
                    ff.isStopped = false;
                    ff.destination = spotPNJ.transform.position;
                    ff.speed = 8f;
                    //TheNPC.GetComponent<Animation>().Play("Walk");
                    //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, FollowSpeed);
                }
                else if (distance <= ff.stoppingDistance)
                {
                    //ff.speed = 0;
                    //ff.isStopped = true;
                    StartCoroutine(Recommence());
                    //TheNPC.GetComponent<Animation>().Play("Idle");
                }

            }
        }
        else
        {
            if (InitialSpawn)
            {
                ResetMe();
            }
            else
            {
                InitialSpawn = GameObject.Find("SpawnPNJ").GetComponent<Transform>();
            }
           
        }
    }

    public void ResetMe()
    {
        this.transform.position = InitialSpawn.position;
        this.transform.rotation = InitialSpawn.rotation;
    }
    IEnumerator Recommence()
    {
        yield return new WaitForSeconds(0.5f);
        ff.isStopped = false;
    }
    void FaceTarget()
    {
        Vector3 direction = (spotPNJ.transform.position - transform.position); //.normalized
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (InteragirText != null && !done)
            {
                InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour intéragir (" + this.gameObject.name + ")";
                InteragirText.SetActive(true);
                
            }
            canInteract = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (InteragirText != null && !done)
            {
                InteragirText.SetActive(false);
                
            }
            canInteract = false;
        }
    }
}
