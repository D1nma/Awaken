using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;


public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public Vector3 lastCheckPointPos;
    public Transform[] spawnPosition;
    public GameObject playerInstance;
    public GameObject InteragirText,Tips,TextWarning,Staminabar;
    public CharacterController cc;
    public bool OutofP;
    float oldValueInt;
    public LampeHuile LH;
    Volume volume;
    private Vignette vg;
    public GameObject Player;
    public UIManager ui;
    private Warning warning;
    float time;
    public static bool gameOver = false;
    public float deadDelay = 2f; //un delay l'animation de la mort?
    public CinemachineFreeLook cam;
    public Transform spawnPos;
    public bool testeur = false;

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
        gameOver = false;
        OutofP = false;
        volume = GameObject.FindGameObjectWithTag("PostP").GetComponent<Volume>();
        if (!cam)
        {
            cam = GameObject.Find("Third Person Camera").GetComponent<CinemachineFreeLook>();
        }
        spawnPos = spawnPosition[0];
        if (!ui)
        {
            ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        }
        if (!LH)
        {
            LH = GameObject.Find("Lampe à huile").GetComponent<LampeHuile>();
        }
        if (!warning)
        {
            warning = GameObject.Find("Warning").GetComponent<Warning>();
        }
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        volume.profile.TryGet(out vg); //on récupère la vignette
        oldValueInt = vg.intensity.value; //on garde en mémoire la valeur par défault
        lastCheckPointPos = spawnPos.transform.position; //on prend la position du spawn de base et non les checkpoints
        SpawnPlayer(); //on spawn pour la première fois 
    }

    public void SpawnPlayer() //Début, Mort ou sorti de map, spawn sur checkpoint le plus proche
    {
        if (!gameOver)
        {
            if (!Player && !testeur)
                Player = Instantiate(playerInstance, spawnPos.position, spawnPos.rotation);
            else if(!Player && testeur)
            {
                Debug.LogWarning("Pose le joueur où tu veux avant de jouer..");
            }
            cam.Follow = Player.transform;
            cam.LookAt = Player.transform;
            cc = Player.GetComponent<CharacterController>();
            if (warning.warningText)
                warning.warningText.SetActive(false);
        }

    }
    public void Replace()
    {
        if (Player.transform.position == lastCheckPointPos)
        {
            PlayersController.canControl = true;
            cc.enabled = true;
            OutOfPosition.enter = false;
            ui.transition.SetTrigger("End");
            RenderSettings.fogEndDistance = Warning.oldValue;
            Warning.StartFog = false;
            if (warning.warningText)
                warning.warningText.SetActive(false);
        }
        else
        {
            ui.transition.SetTrigger("Start");
            cc.enabled = false;
            Player.transform.position = lastCheckPointPos;
        }
    }
    public void ReplaceGO()
    {
        
        if (Player.transform.position == lastCheckPointPos)
        {
            PlayersController.canControl = true;
            cc.enabled = true;
            OutOfPosition.enter = false;
            ui.transition.SetTrigger("End");
            RenderSettings.fogEndDistance = Warning.oldValue;
            Warning.StartFog = false;
            if (warning.warningText)
                warning.warningText.SetActive(false);
        }
        else
        {
            ui.transition.SetTrigger("Start");
            cc.enabled = false;
            Player.transform.position = lastCheckPointPos;
        }
    }


    void Update()
    {
        if(cam.Follow == null || cam.LookAt == null)
        {
            cam.Follow = Player.transform;
            cam.LookAt = Player.transform;
        }
        if (OutofP)
        {
            vg.intensity.value = 0.5f;
        }
        else
        {
            vg.intensity.value = oldValueInt;
        }
        if (!cam)
        {
            cam = GameObject.Find("Third Person Camera").GetComponent<CinemachineFreeLook>();
        }
        if (!Player)
        {
            spawnPos = GameObject.Find("SpawnPlayer").transform;
            spawnPos.position = lastCheckPointPos;
            Player = GameObject.FindGameObjectWithTag("Player");
            SpawnPlayer();
        }
        time += Time.deltaTime;
        ui.UpdateTime((int)time);
        if (gameOver == true)
        {
            //Debug.Log("Game Over");
            PlayersController.canControl = false;
            AkSoundEngine.PostEvent("StopAll", gameObject);
            EndGame();
        }
    }

    public void EndGame()
    {
        ui.DeadMenu();
    }
}
