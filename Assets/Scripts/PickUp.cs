using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    float throwForce = 700;
    Vector3 objectPost;
    float distance;

    public bool canHold = true;
    public GameObject item;
    public GameObject hand;
    public bool isHolding = false;

    void Start()
    {
        StartCoroutine(SearchingHand());
    }

    IEnumerator SearchingHand()
    {
        yield return new WaitForSeconds(1);
        hand = GameObject.Find("Hand");
        if (!hand)
        {
            Debug.Log("Trouve pas la main");
        }
    }

    void Update()
    {
        if (hand)
        {
            distance = Vector3.Distance(item.transform.position, hand.transform.position);
            if (distance >= 2f)
            {
                isHolding = false;
            }

            if (isHolding == true)
            {
                item.GetComponent<Rigidbody>().velocity = Vector3.zero;
                item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                item.transform.SetParent(hand.transform);

                if (Input.GetMouseButtonDown(1) || Input.GetButtonDown("Fire2"))
                {
                    item.GetComponent<Rigidbody>().AddForce(hand.transform.forward * throwForce);
                    isHolding = false;
                }
            }
            else
            {
                objectPost = item.transform.position;
                item.transform.SetParent(null);
                item.GetComponent<Rigidbody>().useGravity = true;
                item.transform.position = objectPost;
            }
            if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1"))
            {
                if (distance <= 2f)
                {
                    isHolding = true;
                    item.GetComponent<Rigidbody>().useGravity = false;
                    item.GetComponent<Rigidbody>().detectCollisions = true;
                }
            }
            if (Input.GetMouseButtonUp(0) || Input.GetButtonUp("Fire1"))
            {
                if (distance <= 2f)
                {
                    isHolding = false;
                }
            }
        }
        

    }
}
