using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnnemy : MonoBehaviour
{
    SpawnEnemy spawnEnemy;
    public int difficulty = 1;

    void Start()
    {
        StartCoroutine(AfterInstance());
    }

    IEnumerator AfterInstance()
    {
        yield return new WaitForSeconds(1);
        spawnEnemy = GameObject.FindGameObjectWithTag("Player").GetComponent<SpawnEnemy>();
        if (!spawnEnemy)
        {
            Debug.LogWarning("Il est ou le joueur? Tag le !");
        }
    }


    void OnTriggerEnter(Collider player)
    {
        if (player.tag == "Player")
        {
            Debug.Log("Entrer Zone de danger");
            if (!spawnEnemy)
            {
                spawnEnemy = player.transform.gameObject.GetComponent<SpawnEnemy>();
            }
            if (spawnEnemy != null)
            {
                if (difficulty <= 1)
                {
                    spawnEnemy.spawnTime = 40f;
                }
                if (difficulty == 2)
                {
                    spawnEnemy.spawnTime = 30f;
                }
                if (difficulty == 3)
                {
                    spawnEnemy.spawnTime = 20f;
                }
                if (difficulty >= 4)
                {
                    spawnEnemy.spawnTime = 10f;
                }
            }
            spawnEnemy.spawnOk = true;
        }
    }

    void OnTriggerExit(Collider player)
    {
        if (player.tag == "Player")
        {
            Debug.Log("Sortie Zone de danger");
            spawnEnemy.spawnOk = false;
        }
    }
}
