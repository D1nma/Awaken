using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersoInteraction : MonoBehaviour
{
    public GameObject InteragirText, player;
    private bool fait = false, canInteract = false, startTiming = false, bouge = false, setup = false;
    float time = 0f, oldDistance = 0f;
    CharacterController cc;
    public UIManager ui;
    private Vector3 target;
    public float speed = 1.0f;
    public Inventaire invent;
    Vector3 playerPos;
    public GameObject Tips;
    // Start is called before the first frame update

    private static PersoInteraction instance;
    void Awake()
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
        fait = false;
        setup = false;
        StartCoroutine(AfterInstance());
        bouge = false;
        if (!ui)
        {
            ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        }
        if (!Tips)
        {
            Debug.LogWarning("Ajouter les parametres necessaires au fonctionnement dans l'inspecteur..");
        }

    }

    IEnumerator AfterInstance()
    {
        yield return new WaitForSeconds(5);
        player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
        {
            Debug.Log("Il est ou le joueur? Tag le !");
        }
        cc = player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        else if (!invent)
        {
            invent = GameObject.Find("Inventaire").GetComponent<Inventaire>();
        }
        else if (!ui)
        {
            ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        }
        if (bouge)
        {
            if (player && !setup)
            {
                playerPos = player.transform.position;
                target = new Vector3(transform.position.x, playerPos.y, transform.position.z);
                Vector3 direction = (transform.position - player.transform.position); //.normalized
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookRotation, Time.deltaTime * 10f);
                //Debug.Log(target);
                
            }
            float step = speed * Time.deltaTime;
            ui.transition.SetTrigger("Start");
            cc.enabled = false;
            player.transform.position = Vector3.MoveTowards(player.transform.position, target, step);
            PlayersController.moving = true;
            float distance = Vector3.Distance(player.transform.position, transform.position);
            invent.DialogueClé.SetActive(true);
            if (distance == oldDistance)
            {
                setup = true;
                bouge = false;
                invent.canne = false;
                PlayersController.moving = false;
                StartCoroutine(BirdKey(2));
            }
            oldDistance = distance;
        }
        else
        {
            ui.transition.SetTrigger("End");

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

    private IEnumerator BirdKey(float duree)
    {
        //animator.SetBool("Bird", true);
        yield return new WaitForSeconds(duree);
        invent.First = true;
        invent.key = true;
        PlayersController.canControl = true;
        cc.enabled = true;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (InteragirText != null && !fait && invent.Keyvolee && Appat.fait)
            {
                InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour intéragir (" + this.gameObject.name + ")";
                InteragirText.SetActive(true);
                if (invent.canne)
                {
                    invent.DialoguePeche2.SetActive(true);
                    invent.ApresRocher=true;
                }
                canInteract = true;
            }
            else if (invent.Keyvolee && Appat.fait == false)
            {
                InteragirText.gameObject.GetComponent<Text>().text = "Il faut placer la boîte d'appât pour attirer l'oiseau";
                InteragirText.SetActive(true);
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
