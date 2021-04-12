using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPong : MonoBehaviour
{
    public float speed=0.1f, valueMax=0.2f,valueMin=0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material mat = renderer.material;

        float emission = valueMin + Mathf.PingPong(Time.time*speed, valueMax-valueMin);
        Color baseColor = Color.white;

        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        mat.SetColor("_EmissionColor", finalColor);
    }
}
