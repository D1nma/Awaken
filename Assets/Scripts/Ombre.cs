using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ombre : MonoBehaviour
{
    Animator _animator;
    GameObject player;
    NavMeshAgent enemy;
    public float LifeTime = 3000f;

    private bool immortal = false;
    float fastSpeed;

    public bool idleOne = false, canTouch;
    private bool canHurt = false;
    bool doOnce;
    private bool follow = true;
    public static bool see = false;

    public int degat = 10;
    public float MaxDist = 0.1f;
    public float MinDist = 5;
    public float lookRadius = 10f, offsetRay = 1;
    private float time;
    Vector3 trait;

    public Transform[] pathPoints;
    int currentDestinationIndex = 0, attaque = 0;



    void Start()
    {

        _animator = GetComponent<Animator>();
        StartCoroutine(AfterInstance());
        enemy = GetComponent<NavMeshAgent>();
        _animator.SetBool("Chasing", false);
        if (pathPoints.Length != 0)
        {
            currentDestinationIndex = (currentDestinationIndex + 1) % pathPoints.Length;
        }
        fastSpeed = enemy.speed;

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
        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("Chasing", false);
        enemy.isStopped = false;
        follow = true;
    }
    IEnumerator Mort()
    {
        AkSoundEngine.PostEvent("Frisson_Start", gameObject);
        yield return new WaitForSeconds(0.2f);
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= enemy.stoppingDistance && canHurt)
            GameManager.gameOver = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (!doOnce)
        {
            AkSoundEngine.PostEvent("Ombre", gameObject);
            doOnce = true;
        }
        if (player && GameManager.gameOver != true)
        {
            if (PlayersController.canControl == false || PlayersController.wakeUp == true)
            {
                canTouch = false;
            }
            else
            {
                canTouch = true;
            }
            if (canTouch)
            {



                trait = this.transform.position;
                trait.y = this.transform.position.y + offsetRay;
                var ray = new Ray(trait, this.transform.forward);
                Vector3 direction = player.transform.position - trait;
                RaycastHit hit;
                Ray ray2 = new Ray(trait, direction);
                Debug.DrawRay(ray2.origin, ray2.direction * 2f, Color.green);
                RaycastHit hitinfo;
                float angle = Vector3.Angle(direction, transform.forward);
                Debug.DrawRay(trait, this.transform.forward * 1, Color.white);
                if (Physics.Raycast(trait, direction, out hitinfo, 5f))
                {
                    //Debug.Log(hitinfo.transform.name);
                    float distance = Vector3.Distance(hitinfo.transform.gameObject.transform.position, transform.position);
                    if (hitinfo.transform.gameObject.tag != "MainCamera"
                        && hitinfo.transform.gameObject.tag != "point"
                        && hitinfo.transform.gameObject.tag != null
                        && hitinfo.transform.gameObject.tag != "Spawn"
                        && hitinfo.transform.gameObject.tag != "Decors"
                        && hitinfo.transform.gameObject.tag != "Ground"
                        && hitinfo.transform.gameObject.tag != "UI"
                        && hitinfo.transform.gameObject.tag != "GM"
                        && hitinfo.transform.gameObject.tag != "Invisible")
                    {

                        if (hitinfo.transform.gameObject.tag == "Player")
                        {

                            //Debug.Log("Alyx c'est mon 4h");
                            if (Physics.Raycast(ray, out hit, 30f) && angle < 160 || distance < 2f)
                            {
                                see = true;
                                canHurt = true;
                                //Debug.Log("Alyx c'est mon 6h");
                            }
                            else
                            {
                                canHurt = false;

                            }
                        }
                    }
                    else
                    {
                        see = false;
                        Debug.Log("Je vois rien");
                        //follow = false;
                        /*enemy.isStopped = true;
                        if (pathPoints.Length > 1)
                        {
                            StartAgent();
                        }
                        StartCoroutine(Recommence());*/
                    }
                }
            }
        }

        time += Time.deltaTime;
        if (LifeTime > 0 && !immortal) { LifeTime -= time; }
        if (idleOne && follow)
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

        else if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player != null && follow == true)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            //Debug.Log(enemy.stoppingDistance);
            if (distance <= lookRadius && see)
            {
                //transform.LookAt(player.transform);
                enemy.destination = player.transform.position;
                FaceTarget();
                enemy.speed = fastSpeed;
                _animator.SetFloat("speedMultiplier", 1.2f);
                if (distance <= enemy.stoppingDistance)
                {
                    if (canHurt && GameManager.gameOver == false)
                    {
                        AkSoundEngine.PostEvent("Dead", gameObject);
                        attaque = Random.Range(0, 2);
                        _animator.SetInteger("Attaque", attaque);
                        Debug.Log("touché");
                        _animator.SetBool("Chasing", true);

                        StartCoroutine(Mort());
                    }
                    enemy.ResetPath();
                    enemy.isStopped = true;
                    follow = false;
                    StartCoroutine(Recommence());
                }

            }
            else
            {
                _animator.SetFloat("speedMultiplier", 1f);
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
