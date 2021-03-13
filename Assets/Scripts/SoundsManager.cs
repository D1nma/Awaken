using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager instance;

    [SerializeField] public AudioMixer mixer;

    [SerializeField] AudioSource ambiance, ombre;

    public Sound[] sounds;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); //la cam gameManager ne se met pas sur la nouvelle scène
        }

        /*foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
        }*/
    }
    void Start()
    {
        //Play("MainTheme");
        if (SceneManager.GetActiveScene().buildIndex == 1) { AkSoundEngine.PostEvent("Ambiance", gameObject); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void Play(string sound)
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
