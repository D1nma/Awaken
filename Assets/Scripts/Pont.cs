using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pont : MonoBehaviour
{
    private GameObject player;
    public GameObject point;
    public Inventaire invent;
    CharacterController cc;
    public GameObject DialoguePont;
    public bool pass=false;
    private bool setup=false;
    float speed = 1f;
    float oldDistance = 0f;
    private Vector3 target;
    Vector3 playerPos;

    IEnumerator Setup()
    {
        yield return new WaitForSeconds(1);
        if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        cc = player.GetComponent<CharacterController>();
    }
    void Start()
    {
        pass = false;
        StartCoroutine(Setup());
    }

    // Update is called once per frame
    void Update()
    {
        if (!invent || !player || !cc)
        {
            StartCoroutine(Setup());
        }
        else
        {
            if (invent.Keyvolee)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                if (pass)
                {
                    if (player && !setup)
                    {
                        playerPos = player.transform.position;
                        target = new Vector3(point.transform.position.x, playerPos.y, point.transform.position.z);
                        Vector3 direction = (point.transform.position - player.transform.position); //.normalized
                        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookRotation, Time.deltaTime * 100f);
                        setup = true;
                    }
                    float step = speed * Time.deltaTime;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, target, step);
                    PlayersController.moving = true;
                    float distance = Vector3.Distance(player.transform.position, transform.position);
                    if (distance == oldDistance)
                    {
                        PlayersController.moving = false;
                        if (setup && pass)
                        {
                            PlayersController.canControl = true;
                            cc.enabled = true;
                            setup = false;
                            pass = false;
                        }
                    }
                    oldDistance = distance;
                }
                
                this.gameObject.SetActive(true);
            }
        }
        
    }
    void OnTriggerEnter(Collider player)
    {

        if(player.tag == "Player")
        {
            if (invent.Keyvolee == false)
            {
                DialoguePont.SetActive(true);
                PlayersController.canControl = false;
                cc.enabled = false;
                pass = true;
            }
            else
            {
                DialoguePont.SetActive(true);
            }
        }
    
    
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            pass = false;
        }
    }

}
