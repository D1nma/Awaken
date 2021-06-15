using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager instance;
    private bool runOnce=false;

    /*[SerializeField] public AudioMixer mixer;

    [SerializeField] AudioSource ambiance, ombre;*/

    /*public Sound[] sounds;*/

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        /*foreach (Sound s in sounds) //Sans utiliser Wwise
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
        }*/
    }
    void Start()
    {
        //Play("MainTheme"); //Sans utiliser Wwise
        //if (SceneManager.GetActiveScene().buildIndex == 1) { AkSoundEngine.PostEvent("Ambiance", gameObject); }
        runOnce = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!runOnce)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0) { AkSoundEngine.PostEvent("Menu_Start", gameObject); }
            if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2) { AkSoundEngine.PostEvent("Wind_loop_start", gameObject); AkSoundEngine.PostEvent("Birds_Start", gameObject); AkSoundEngine.PostEvent("Lvl1_Start", gameObject); }
            runOnce = true;
        }
    }

    /*public void Play(string sound) //Sans utiliser Wwise
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if(s == null)
        {
            Debug.LogWarning("Sound not found!");
            return;
        }

        //s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f)) ;
        //s.source.pitch = s / pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        if (UIManager.GameIsPaused)
        {
            s.source.pitch *= .5f;
        }

        s.source.Play();
    }

    public void Ambiance()
    {
        ambiance.Play();
    }
    public void OmbreSon()
    {
        ombre.Play();
    }*/
}
