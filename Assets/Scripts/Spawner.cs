using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Player player;
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    public float startingTimeBtw;
    public float currTimeBtw;
    private bool onCooldown = false;

    // Start is called before the first frame update
    void Start()
    {
        int count = transform.childCount;
        spawnPoints = new Transform[count];
        for(int i = 0; i < count; i++){
            spawnPoints[i] = transform.GetChild(i);
        }

        currTimeBtw = startingTimeBtw;
    }

    // Update is called once per frame
    void Update()
    {
        currTimeBtw = startingTimeBtw - player.enemyKillCount * 0.1f;
        if (currTimeBtw < .25f)
            currTimeBtw = .25f;

        if (!onCooldown)
            SpawnEnemy();
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, transform.rotation);
        StartCoroutine(StartCooldown());
    }

    public IEnumerator StartCooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(currTimeBtw);
        onCooldown = false;
    }
}
