using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LampeHuile : MonoBehaviour
{
    private UIManager ui;
    public static LampeHuile instance;
    public Slider huileBar;

    public GameObject InteragirText;
    public GameObject Tips;
    public GameObject lumiere;
    public GameObject main;
    public GameObject ObjNbRecharges;
    public GameObject LAmoi;
    public GameObject prefab;
    public int nbRecharges = 1;
    private int maxHuile = 100;

    public float currentHuile = 25;
    private int tipInt = 0;
    private float consume = 0.005f;
    [HideInInspector]
    public bool use;
    float time;
    [HideInInspector]
    public bool dispo;
    [HideInInspector]
    public bool EnMain, tipBool, startTiming = false, consomme = false;


    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        tipBool = false;
        if (!ui)
        {
            ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        }
        if (!EnMain)
        {
            dispo = false;
        }
        huileBar.maxValue = maxHuile;
        huileBar.value = currentHuile;
        ui.huile.SetActive(false);
        LAmoi = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
        ObjNbRecharges.gameObject.GetComponent<Text>().text = nbRecharges.ToString();
        if (main == null)
        {
            main = GameObject.Find("Hand");
        }
        if (EnMain)
        {
            if(ObjNbRecharges != null){
                ObjNbRecharges.SetActive(true);
            }
            useHuile();
            if (currentHuile == 0)
            {
                ui.Etathuile[0].SetActive(true);
                ui.Etathuile[1].SetActive(false);
            }
            if (currentHuile > 0)
            {
                ui.Etathuile[1].SetActive(true);
                ui.Etathuile[0].SetActive(false);
                ui.Etathuile[2].SetActive(false);
            }
            if (currentHuile > 25)
            {
                ui.Etathuile[2].SetActive(true);
                ui.Etathuile[1].SetActive(false);
                ui.Etathuile[3].SetActive(false);
            }
            if (currentHuile > 50)
            {
                ui.Etathuile[3].SetActive(true);
                ui.Etathuile[2].SetActive(false);
                ui.Etathuile[4].SetActive(false);
            }
            if (currentHuile > 75)
            {
                ui.Etathuile[4].SetActive(true);
                ui.Etathuile[3].SetActive(false);
                ui.Etathuile[5].SetActive(false);
            }
            if (currentHuile == 100)
            {
                ui.Etathuile[5].SetActive(true);
                ui.Etathuile[4].SetActive(false);
            }
        }
        if (!EnMain)
        {
            ui.huile.SetActive(false);
            ui.Etathuile[0].SetActive(false);
            ui.Etathuile[1].SetActive(false);
            ui.Etathuile[2].SetActive(false);
            ui.Etathuile[3].SetActive(false);
            ui.Etathuile[4].SetActive(false);
            ui.Etathuile[5].SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.E) && dispo)
        {
            LAlampe();
            InteragirText.SetActive(false);
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
        if (InteragirText == null)
        {
            InteragirText = GameObject.Find("InteragirText");
        }
        if (Tips == null)
        {
            Tips = GameObject.Find("Tips");
        }
        if (currentHuile > 100)
        {
            currentHuile = 100;
        }
        if (Input.GetKeyUp(KeyCode.F) && EnMain)
        {
            if (nbRecharges > 0 && currentHuile < 80)
            {
                Recharge();
                nbRecharges -= 1;
            }
            else
            {
                if (currentHuile > 80)
                {
                    Tips.gameObject.GetComponent<Text>().text = "Vous avez plus de 80% d'huile !";
                    Tips.SetActive(true);
                    startTiming = true;
                }
                else
                {
                    Tips.gameObject.GetComponent<Text>().text = "Vous n'avez plus de recharge !";
                    Tips.SetActive(true);
                    startTiming = true;
                }

            }

        }
        if (!EnMain)
        {
            lightDown();
        }
        if (currentHuile <= 0)
        {
            lightDown();
            StopHuile();
        }
        if (consomme)
        {
            if (currentHuile > 0)
            {
                currentHuile -= consume;
                huileBar.value = currentHuile;
                lightUp();

            }
            else { StopHuile(); lightDown(); }
        }
    }

    public void LAlampe()
    {
            EnMain = true;
            lightUp();
            ui.huile.SetActive(true);
            ui.Etathuile[0].SetActive(true);
            if (tipInt == 0)
            {
                tipInt = 1;
                if (Tips != null)
                {
                    Tips.gameObject.GetComponent<Text>().text = "Appuyer sur F pour recharger";
                    Tips.SetActive(true);
                    startTiming = true;

                }
            }
            transform.position = main.transform.position;
            transform.parent = main.transform;
    }

    void OnTriggerEnter(Collider player)
    {

        if (player.tag == "Player" && !EnMain)
        {
            if (InteragirText != null)
            {
                InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur E pour intéragir ("+this.gameObject.name+")";
                InteragirText.SetActive(true);
                dispo = true;
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
                dispo = false;
            }
        }
    }
    void useHuile()
    {
        if (currentHuile - consume >= 0)
        {
            use = true;
            consomme = true;
        }
    }
    public void StopHuile()
    {
        use = false;
    }
    void Recharge()
    {
        currentHuile += 25;
    }
    void lightUp()
    {
        if (EnMain)
        {
            lumiere.SetActive(true);
        }
    }
    void lightDown()
    {
        lumiere.SetActive(false);
    }
}
    