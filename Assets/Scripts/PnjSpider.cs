using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PnjSpider : MonoBehaviour
{
    Animator _animator;
    private NavMeshAgent _agent;
    public GameObject player;
    public float PnjDistanceRun = 4f;
    float oldSpeed, x, z, time, timeAlea;
    float speedNav,oldSpeedNav;
    private bool PowerOn,walk;
    bool timeGo,Destination, setTime = false;
    Vector3 randomPosition;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        StartCoroutine(AfterInstance());
        oldSpeed = _agent.speed;
        _agent.isStopped = true;
        speedNav = _animator.GetFloat("Speed");
        oldSpeedNav = speedNav;
        timeGo = false;
        time = 0;
        timeAlea = Random.Range(2f, 6f);
        setTime = true;
        PowerOn = false;
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

    // Update is called once per frame
    void Update()
    {
        if (PowerOn)
        {
            if (_agent.velocity.magnitude < 0.15f)
            {
                _animator.SetBool("Walk", false);
                walk = false;
            }
            else
            {
                _animator.SetBool("Walk", true);
                walk = true;
            }
            /*var ray = new Ray(this.transform.position, this.transform.forward/2);
       Debug.DrawRay(this.transform.position, this.transform.forward/2, Color.white);
       RaycastHit hit;
       if (Physics.Raycast(ray, out hit, 5f))
       {
           _agent.ResetPath();
           Debug.Log("Je bloque");
           Vector3 dirToPlayer = transform.position - hit.transform.position;
           Vector3 newPos = transform.position + dirToPlayer;
           _agent.SetDestination(newPos);
       }*/
            //Debug.Log(_agent.destination);
            //Debug.Log(_agent.isStopped);
            if (player)
            {
                
                float distance = Vector3.Distance(transform.position, player.transform.position);

                if (distance < PnjDistanceRun)
                {
                    _agent.speed = _agent.speed * 2;
                    _animator.SetFloat("Speed", speedNav * 2);
                    Vector3 dirToPlayer = transform.position - player.transform.position;
                    Vector3 newPos = transform.position + dirToPlayer;

                    _agent.SetDestination(newPos);
                    _agent.isStopped = false;
                }
                else
                {
                    _agent.speed = oldSpeed;
                    speedNav = oldSpeedNav;
                    _animator.SetFloat("Speed", speedNav);
                }
                if (distance < PnjDistanceRun / 2)
                {
                    _agent.speed = _agent.speed * 4;
                    _animator.SetFloat("Speed", speedNav * 4);
                }
                else
                {
                    _agent.speed = oldSpeed;
                    speedNav = oldSpeedNav;
                    _animator.SetFloat("Speed", speedNav);
                }
            }
            if (timeGo)
            {
                //Debug.Log("le temps tourne");
                time += Time.deltaTime;
                //Debug.Log(timeAlea);
            }
            if (time >= timeAlea)
            {
                //Debug.Log("Pause fini");
                _animator.SetBool("Walk", false);
                timeGo = false;
                time = 0;
                _agent.isStopped = false;
                setTime = false;
                Destination = true;
            }
            if (_agent.isStopped)
            {
                _animator.SetBool("Walk", false);
                TempsAlea();
                timeGo = true;
            }
            if (Destination)
            {
                //Debug.Log("je trouve une destination");
                x = Random.Range(transform.position.x - 10, transform.position.x + 10);
                z = Random.Range(transform.position.z - 10, transform.position.z + 10);
                randomPosition = new Vector3(x, transform.position.y, z);
                _agent.SetDestination(randomPosition);
                _agent.isStopped = false;
                Destination = false;
            }
            if (_agent.remainingDistance >= 0.1f)
            {
                //_animator.SetBool("Walk", true);
                //Debug.Log(_agent.remainingDistance);
            }
            else if (_agent.remainingDistance < 0.1f)
            {
                //Debug.Log("Je suis proche");
                _animator.SetBool("Walk", false);
                _agent.isStopped = true;
            }
        }else
        {
            _animator.SetBool("Walk", false);
        }
       
    }
    void TempsAlea()
    {
        if (!setTime)
        {
            timeAlea = Random.Range(2f, 6f);
            setTime = true;
        }
        
    }
    void OnTriggerEnter(Collider player)
    {

        if (player.tag == "Player")
        {
            PowerOn = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PowerOn = false;
        }
    }
}
