using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LampeHuile : MonoBehaviour
{
    private UIManager ui;
    public static LampeHuile instance;
    public Slider huileBar;
    GameManager gm;
    public GameObject verre;
    Renderer renderer;
    Material [] mats;
    Material mat;
    Color oldValue;
    public GameObject InteragirText;
    public GameObject Tips;
    public GameObject lumiere;
    public GameObject main;
    public GameObject ObjNbRecharges;
    public GameObject prefab;
    WatchMe watchMe;
    public int nbRecharges = 3;
    private int maxHuile = 100;
    float oldValueFog;
    public float FogStartDistance = 50f;
    public float currentHuile = 50;
    private int tipInt = 0;
    private float consume = 0.1f;
    [HideInInspector]
    public bool use;
    float time;
    [HideInInspector]
    public bool dispo;
    [HideInInspector]
    public bool EnMain;
    [HideInInspector]
    private bool startTiming = false, consomme = false;


    private void Awake()
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
    // Start is called before the first frame update
    void Start()
    {
        renderer = verre.GetComponent<Renderer>();
        mats = renderer.materials;
        mat = mats[1];
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        oldValue = mat.GetColor("_EmissionColor");
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
        oldValueFog = RenderSettings.fogStartDistance;
        if (!watchMe)
        {
            watchMe = GetComponent<WatchMe>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!ObjNbRecharges)
        {
            ObjNbRecharges = GameObject.Find("ObjNbRecharge");
        }
        else
        {
            ObjNbRecharges.gameObject.GetComponent<Text>().text = nbRecharges.ToString();
        }
        if (main == null)
        {
            main = GameObject.Find("Hand");
        }else if (!gm)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        }
        else if (!InteragirText)
        {
            InteragirText = gm.InteragirText;
        }
        else if (Tips == null)
        {
            Tips = gm.Tips;
        }
        if (EnMain)
        {
            if(watchMe.watchMe == true)
            {
                watchMe.watchMe = false;
            }
            RenderSettings.fogStartDistance = FogStartDistance;
            if (ObjNbRecharges != null)
            {
                ObjNbRecharges.SetActive(true);
            }
            useHuile();
            if (currentHuile == 0)
            {
                ui.Etathuile[0].SetActive(true);
                ui.Etathuile[1].SetActive(false);
            }
            else if (currentHuile > 0)
            {
                ui.Etathuile[1].SetActive(true);
                ui.Etathuile[0].SetActive(false);
                ui.Etathuile[2].SetActive(false);
            }
            else if (currentHuile > 25)
            {
                ui.Etathuile[2].SetActive(true);
                ui.Etathuile[1].SetActive(false);
                ui.Etathuile[3].SetActive(false);
            }
            else if (currentHuile > 50)
            {
                ui.Etathuile[3].SetActive(true);
                ui.Etathuile[2].SetActive(false);
                ui.Etathuile[4].SetActive(false);
            }
            else if (currentHuile > 75)
            {
                ui.Etathuile[4].SetActive(true);
                ui.Etathuile[3].SetActive(false);
                ui.Etathuile[5].SetActive(false);
            }
            else if (currentHuile == 100)
            {
                ui.Etathuile[5].SetActive(true);
                ui.Etathuile[4].SetActive(false);
            }
        }
        else if (!EnMain)
        {
            RenderSettings.fogStartDistance = oldValueFog;
            ui.huile.SetActive(false);
            ui.Etathuile[0].SetActive(false);
            ui.Etathuile[1].SetActive(false);
            ui.Etathuile[2].SetActive(false);
            ui.Etathuile[3].SetActive(false);
            ui.Etathuile[4].SetActive(false);
            ui.Etathuile[5].SetActive(false);
            ObjNbRecharges.SetActive(false);
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
        /*if (!EnMain)
        {
            //lightDown();
        }*/
        if (currentHuile <= 0)
        {
            lightDown();
            StopHuile();
        }
        if (consomme)
        {
            if (currentHuile > 0)
            {
                currentHuile -= consume * Time.deltaTime;
                PlayerStress.LH = true;
                huileBar.value = currentHuile;
                lightUp();

            }
            else { StopHuile(); lightDown(); PlayerStress.LH = false; }
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
        StartCoroutine(GetLampe()); // après quelque milli sec pour que le bras soit déjà en position

    }

    void OnTriggerEnter(Collider player)
    {

        if (player.tag == "Player" && !EnMain)
        {
            if (InteragirText != null)
            {
                InteragirText.gameObject.GetComponent<Text>().text = "Appuie sur “E” ou “Triangle” pour intéragir (" + this.gameObject.name + ")"; //pour prendre/interagir avec un objet “E” ou “△”
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
        currentHuile = 100;
    }
    void lightUp()
    {
        if (EnMain)
        {
            lumiere.SetActive(true);
            mat.SetColor("_EmissionColor", oldValue);
        }
    }
    void lightDown()
    {
        lumiere.SetActive(false);
        mat.SetColor("_EmissionColor", new Color(0,0,0,0));
    }
    private IEnumerator GetLampe()
    {
        yield return new WaitForSeconds(0.3f);
        transform.position = main.transform.position;
        transform.parent = main.transform;
        
    }
}
