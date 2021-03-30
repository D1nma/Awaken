using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersoInteraction : MonoBehaviour
{
    public GameObject InteragirText, player;
    private bool fait = false, canInteract = false, startTiming = false, bouge = false, setup = false;
    float time = 0f, oldDistance = 0f;
    private Vector3 target;
    public float speed = 1.0f;
    public Inventaire invent;
    Vector3 playerPos;
    public GameObject Tips;
    // Start is called before the first frame update
    void Start()
    {
        fait = false;
        setup = false;
        StartCoroutine(AfterInstance());
        bouge = false;

    }

    IEnumerator AfterInstance()
    {
        yield return new WaitForSeconds(5);
        player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
        {
            Debug.Log("Il est ou le joueur? Tag le !");
        }
        if (!Tips)
        {
            Debug.LogWarning("Ajouter les parametres necessaires au fonctionnement dans l'inspecteur..");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }
        if (bouge)
        {
            if (player && !setup)
            {
                playerPos = player.transform.position;
                target = new Vector3(transform.position.x, playerPos.y, transform.position.z);
                Vector3 direction = (transform.position - player.transform.position); //.normalized
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookRotation, Time.deltaTime * 50f);
                //Debug.Log(target);
                setup = true;
            }
            float step = speed * Time.deltaTime;
            player.transform.position = Vector3.MoveTowards(player.transform.position, target, step);
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance == oldDistance)
            {
                bouge = false;
                PlayersController.canControl = true;
                invent.canne = false;
            }
            oldDistance = distance;
        }
        if (Input.GetKeyUp(KeyCode.E) && canInteract == true)
        {
            if (invent.canne == true)
            {
                InteragirText.SetActive(false);
                PlayersController.canControl = false;
                fait = true;
                bouge = true;
            }
            else
            {
                startTiming = true;
                Tips.SetActive(true);
                InteragirText.SetActive(false);
                Tips.gameObject.GetComponent<Text>().text = "Il faut la canne à pêche!";
            }

        }
        if (startTiming)
        {
            time += Time.deltaTime;
        }
        if (time >= 4)
        {
            Tips.SetActive(false);
            time = 0;
            startTiming = false;
        }
        if (fait)
        {
            canInteract = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (InteragirText != null && !fait)
            {
                InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour intéragir (" + this.gameObject.name + ")";
                InteragirText.SetActive(true);
                canInteract = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (InteragirText != null)
            {
                InteragirText.SetActive(false);
                canInteract = false;
            }
        }
    }
}
