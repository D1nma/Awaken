﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public Vector3 lastCheckPointPos;
    public Transform[] spawnPosition;
    public GameObject playerInstance;
    CharacterController cc;
    private LampeHuile LH;
    public GameObject Player;
    public UIManager ui;
    private Warning warning;
    float time;
    public static bool gameOver = false;
    public float deadDelay = 2f; //un delay l'animation de la mort?
    public CinemachineFreeLook cam;
    public Transform spawnPos;

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
        Debug.Log(lastCheckPointPos);
        lastCheckPointPos = spawnPos.transform.position;
        Debug.Log(lastCheckPointPos);
        SpawnPlayer();
    }

    public void SpawnPlayer() //Début, Mort ou sorti de map, spawn sur checkpoint le plus proche
    {

        if (!gameOver)
        {
            if (!Player)
                Player = Instantiate(playerInstance, spawnPos.position, spawnPos.rotation);
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


    void Update()
    {
        if (!Player)
        {
            spawnPos = GameObject.Find("SpawnPlayer").transform;
            spawnPos.position = lastCheckPointPos;
            cam = GameObject.Find("Third Person Camera").GetComponent<CinemachineFreeLook>();
            SpawnPlayer();

        }
        time += Time.deltaTime;
        ui.UpdateTime((int)time);
        if (gameOver == true)
        {
            Debug.Log("Game Over");
            PlayersController.canControl = false;
            EndGame();
        }
    }

    public void EndGame()
    {

        ui.DeadMenu();
        gameOver = false;
    }
}
