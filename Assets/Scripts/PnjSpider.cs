using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PnjSpider : MonoBehaviour
{
    Animator _animator;
    private NavMeshAgent _agent;
    public GameObject player;
    public float PnjDistanceRun=4f;
    float oldSpeed,x,y,z;
    float speed;
    Vector3 randomPosition;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        StartCoroutine(AfterInstance());
        oldSpeed = _agent.speed;
        _agent.isStopped = true;
        speed = _animator.GetFloat("Speed");
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
        var ray = new Ray(this.transform.position, this.transform.forward);
        Debug.DrawRay(this.transform.position, this.transform.forward, Color.white);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f))
        {
            _agent.isStopped = true;
        }
        //Debug.Log(_agent.destination);
        //Debug.Log(_agent.isStopped);
        if (player)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < PnjDistanceRun)
            {
                Vector3 dirToPlayer = transform.position - player.transform.position;
                Vector3 newPos = transform.position + dirToPlayer;

                _agent.SetDestination(newPos);
                _agent.isStopped = false;
            }
            if(distance < PnjDistanceRun / 4)
            {
                _agent.speed = _agent.speed * 2;
            }
            else
            {
                _agent.speed = oldSpeed;
            }
        }
        if (_agent.isStopped)
        {
            x = Random.Range(-25, 26);
            z = Random.Range(-25, 26);
            randomPosition = new Vector3(x, transform.position.y, z);
            _agent.SetDestination(randomPosition);
            _agent.isStopped = false;
        }
        if(_agent.remainingDistance > 0.1f)
        {
            _animator.SetBool("Walk", true);
        }
        else
        {
            _animator.SetBool("Walk", false);
            _agent.isStopped = true;
        }
    }
}
