using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tips : MonoBehaviour
{
    public GameObject tips;
    GameManager gm;
    [TextArea(3,15)]
    public string text;
    float time;
    bool startTiming;
    // Start is called before the first frame update

    private static Tips instance;
    void Awake()
    {
        if (instance == null)
        {

            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (!gm)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!gm)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        }
        if (startTiming)
        {
            time += Time.deltaTime;
        }
        if (time >= 4)
        {
            tips.SetActive(false);
            time = 0;
            startTiming = false;
            Destroy(this.gameObject);
        }
        //Debug.Log(time);

    }
    void OnTriggerEnter(Collider player)
    {

        if (player.tag == "Player")
        {
            tips.gameObject.GetComponent<Text>().text = text;
            tips.SetActive(true);
            startTiming = true;
            time = 0;
        }
        if (player.tag != "Player") { return; }
    }

}
