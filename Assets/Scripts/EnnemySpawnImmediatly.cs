using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemySpawnImmediatly : MonoBehaviour
{

    public GameObject spawnPosition;
    SpawnEnemy spawn;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AfterInstance());
    }
    IEnumerator AfterInstance()
    {
        yield return new WaitForSeconds(1);
        spawn = GameObject.FindGameObjectWithTag("Player").GetComponent<SpawnEnemy>();
        if (!spawn)
        {
            Debug.LogWarning("Il est ou le joueur? Tag le !");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider player)
    {
        if (player.tag == "Player")
        {
            Instantiate(spawn.enemies[Random.Range(0, spawn.enemies.Length - 1)], spawnPosition.transform.position, Quaternion.identity);
        }
    }

    void OnTriggerExit(Collider player)
    {
        Destroy(this.gameObject);
        Destroy(spawnPosition);
    }
}
