using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject[] enemies;

    public bool spawnOk = false;
    public float spawnTime = 10f;
    public bool spawn = true;

    Ombre ombre;

    private float time;
    private Vector3 spawnPosition;
    void Start()
    {
        //InvokeRepeating("Spawn", spawnTime, spawnTime);
        time = spawnTime;
    }

    void Update()
    {
        if (spawnOk)
        {
            Debug.Log("Le spawn Ennemy activé");
            time -= Time.deltaTime;
            //Debug.Log(time);
            if (time <= 0)
            {
                Spawn();
                time = Random.Range(spawnTime+5f,spawnTime+30f);
            }

        }

    }
    public void Spawn()
    {

        //spawnTime = Random.Range(250f, 1000f);
        spawnPosition.x = Random.Range(transform.position.x - 8, transform.position.x + 8);
        spawnPosition.y = 0.5f;
        spawnPosition.z = Random.Range(transform.position.z - 8, transform.position.z + 8);
        Debug.Log("Spawn ennemie :" + spawnPosition);
        Instantiate(enemies[Random.Range(0, enemies.Length - 1)], spawnPosition, Quaternion.identity);
    }
}
