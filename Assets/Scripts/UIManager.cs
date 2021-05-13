using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text time;
    public GameObject optionPanel;
    public GameObject deadPanel;
    public GameObject LoadingScreen;
    public Slider slider;
    public Text progressText;
    public Animator transition;
    public GameObject stamina;
    public GameObject huile;
    public GameObject[] Etathuile;

    public GameObject Checkpoint, Respawn;

    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    private static UIManager instance;

    public void StartGame(int sceneIndex) //Bouton play
    {
        //SceneManager.UnloadSceneAsync("Menu");
        //SceneManager.LoadScene("Partie_1");
        //SceneManager.LoadScene("PersoMove"); // test a changer


        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); Marche bien aussi

        //StartCoroutine(LoadAsynchronously(sceneIndex));
        StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().buildIndex + 1));


    }

    void Start()
    {

    }

    void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Cursor.visible = false;
            if (instance == null)
            {

                instance = this;
                DontDestroyOnLoad(instance);
            }
            else
            {
                Destroy(gameObject);
            }
        }else{
            Cursor.visible = true;
        }

    }

    private void Update()
    {
        if (Cursor.visible == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().buildIndex + 1));
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Quit();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Cursor.visible = false;
                Resume();
            }
            else
            {
                Cursor.visible = true;
                Pause();
            }
        }
    }
    void Pause()
    {
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void Resume()
    {
        Cursor.visible = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }


    IEnumerator LoadAsynchronously(int sceneIndex) //load scene + recup la progress + transition
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        transition.SetTrigger("Start");
        LoadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
            yield return null;
        }
        if (operation.isDone)
        {
            LoadingScreen.SetActive(false);
            transition.SetTrigger("End");
        }
    }


    public void OnFxSliderChanged(float value) //effets
    {
        SoundsManager.instance.mixer.SetFloat("Effets", value);
    }

    public void OnMusicSliderChanged(float value)
    {
        SoundsManager.instance.mixer.SetFloat("Ambiance", value);
    }

    public void OnGeneralSliderChanged(float value)
    {
        SoundsManager.instance.mixer.SetFloat("General", value);
    }


    public void UpdateTime(int nb) //temps de jeu
    {
        if (time)
        {
            time.text = "Timer : " + nb.ToString();
        }

    }
    public void Option()
    {
        Cursor.visible = true;
        if (optionPanel != null)
            optionPanel.SetActive(true);
        Time.timeScale = 0;


    }
    public void Reload()
    {
        if (deadPanel != null)
            deadPanel.SetActive(false);
        Time.timeScale = 1f;
        GameManager.gameOver = false;
        StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().buildIndex));
        Resume();
    }
    public void DeadMenu()
    {
        Cursor.visible = true;
        if (deadPanel != null)
            deadPanel.SetActive(true);
        Time.timeScale = 0.2f;
    }

    public void CloseOption()
    {
        Cursor.visible = false;
        if (optionPanel != null)
            optionPanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void LoadMenu()
    {
        Cursor.visible = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    public void Quit()
    {
        Cursor.visible = true;
        Application.Quit();
    }

}
