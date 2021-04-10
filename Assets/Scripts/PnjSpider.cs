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
    float speedNav;
    bool timeGo,Destination;
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
        timeGo = false;
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
        var ray = new Ray(this.transform.position, this.transform.forward/2);
        Debug.DrawRay(this.transform.position, this.transform.forward/2, Color.white);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f))
        {
            Debug.Log("Je bloque");
            Vector3 dirToPlayer = transform.position - hit.transform.position;
            Vector3 newPos = transform.position + dirToPlayer;
            _agent.SetDestination(newPos);
        }
        //Debug.Log(_agent.destination);
        //Debug.Log(_agent.isStopped);
        if (player)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < PnjDistanceRun)
            {
                _animator.SetBool("Walk", true);
                Vector3 dirToPlayer = transform.position - player.transform.position;
                Vector3 newPos = transform.position + dirToPlayer;

                _agent.SetDestination(newPos);
                _agent.isStopped = false;
            }
            if (distance < PnjDistanceRun / 4)
            {
                _agent.speed = _agent.speed * 2;
                _animator.SetFloat("Speed",speedNav *2);
            }
            else
            {
                _agent.speed = oldSpeed;
                _animator.SetFloat("Speed", speedNav /2);
            }
        }
        if (timeGo)
        {
            time += Time.deltaTime;
        }
        if(time >= timeAlea)
        {
            _animator.SetBool("Walk", false);
            timeGo = false;
            time = 0;
            _agent.isStopped = true;
            Destination = true;
        }
        if (_agent.isStopped)
        {
            _animator.SetBool("Walk", false);
            timeAlea = Random.Range(2f, 6f);
            timeGo = true;
        }
        if (Destination && _agent.isStopped)
        {
            Debug.Log("je trouve une destination");
            x = Random.Range(-10, 10);
            z = Random.Range(-10, 10);
            randomPosition = new Vector3(x, transform.position.y, z);
            _agent.SetDestination(randomPosition);
            _agent.isStopped = false;
            _animator.SetBool("Walk", true);
            Destination = false;
        }
        
        if (_agent.remainingDistance > 0.1f)
        {
            //_animator.SetBool("Walk", true);
        }
        else
        {
            Debug.Log("Je suis proche");
            _animator.SetBool("Walk", false);
            _agent.isStopped = true;   
        }
    }
}
