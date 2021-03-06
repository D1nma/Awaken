using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PnjGrenouille : MonoBehaviour
{
    Animator _animator;
    private NavMeshAgent _agent;
    public GameObject player;
    public float PnjDistanceRun = 4f;
    private bool PowerOn;
    float oldSpeed, x, z, time, timeAlea;
    public float speedNav = 3f,oldSpeedNav;
    bool timeGo, Destination, setTime = false;
    Vector3 randomPosition;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        StartCoroutine(AfterInstance());
        oldSpeed = _agent.speed;
        _agent.isStopped = true;
        _animator.SetFloat("Speed",speedNav);
        oldSpeedNav = speedNav;
        timeGo = false;
        time = 0;
        timeAlea = Random.Range(4f, 15f);
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
            }
            else
            {
                _animator.SetBool("Walk", true);
            }

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
                    _animator.SetBool("Walk", false);
                    _agent.isStopped = true;
                }
                if (distance < PnjDistanceRun / 2)
                {
                    _agent.speed = _agent.speed * 2;
                    _animator.SetFloat("Speed", speedNav * 2);
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
                x = Random.Range(transform.position.x - 2, transform.position.x + 2);
                z = Random.Range(transform.position.z - 2, transform.position.z + 2);
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


        }
        else
        {
            _animator.SetBool("Walk", false);
        }
    }
    void TempsAlea()
    {
        if (!setTime)
        {
            timeAlea = Random.Range(4f, 15f);
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
