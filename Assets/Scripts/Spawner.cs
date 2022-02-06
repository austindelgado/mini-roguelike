using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        int count = transform.childCount;
        spawnPoints = new Transform[count];
        for(int i = 0; i < count; i++){
            spawnPoints[i] = transform.GetChild(i);
        }

        SpawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, transform.rotation);
    }
}
