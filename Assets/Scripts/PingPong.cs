using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPong : MonoBehaviour
{
    Renderer renderer;
    Material mat;
    float emission;
    Color baseColor;
    private bool PowerOn;
    Color finalColor;
    public float speed=0.1f, valueMax=0.2f,valueMin=0.1f;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        mat = renderer.material;
        baseColor = mat.GetColor("_EmissionColor");
        PowerOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PowerOn)
        {
            emission = valueMin + Mathf.PingPong(Time.time * speed, valueMax - valueMin);
            finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
            mat.SetColor("_EmissionColor", finalColor);
        }
    }
    void OnTriggerEnter(Collider player)
    {

        if (player.tag == "Player")
        {
            PowerOn = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PowerOn = false;
        }
    }
}
