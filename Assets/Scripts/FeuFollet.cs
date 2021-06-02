using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeuFollet : MonoBehaviour
{
    bool pieger = true,follow;
    public GameObject player;
    public float TargetDistance;
    public float AllowedDistance = 5;
    //public GameObject TheNPC;
    public float FollowSpeed;
    public RaycastHit shot;
    void Start()
    {
        
    }

    void Update()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (!pieger && follow && player)
        {
            transform.LookAt(player.transform);
            if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out shot))
            {
                TargetDistance = shot.distance;
                if(TargetDistance >= AllowedDistance)
                {
                    FollowSpeed = 1.1f;
                    //TheNPC.GetComponent<Animation>().Play("walk");
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, FollowSpeed);
                }
                else
                {
                    FollowSpeed = 0;
                }

            }
        }
    }
}
