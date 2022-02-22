using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemyPrefab;
    public Transform[] spawnPoints;
        
    private Text scoreText;

    public int waveNum = 0;

    public float scalingMult;
    public float altChance;
    public int enemiesToSpawn;
    private int numRemaining;
    public bool waveActive = false;

    public int enemiesActive;

    public float timeBtw;
    private bool onCooldown = false;

    // Start is called before the first frame update
    void Start()
    {
        int count = transform.childCount;
        spawnPoints = new Transform[count];
        for(int i = 0; i < count; i++){
            spawnPoints[i] = transform.GetChild(i);
        }

        scoreText = GameObject.Find("Score").GetComponent<Text>();
        numRemaining = enemiesToSpawn;

        GameEvents.current.onRoundStart += StartWave;
    }

    // Update is called once per frame
    void Update()
    {
        // There has to be a better way
        if (waveActive && !onCooldown && numRemaining > 0)
            SpawnEnemy();

        // Wave clear
        if (waveActive && enemiesActive == 0)
        {
            GameEvents.current.RoundEnd(waveNum);

            waveActive = false;
        }
    }

    public void StartWave(int round)
    {
        if (!waveActive)
        {
            waveActive = true;
            waveNum++;
            
            scoreText.text = waveNum.ToString();

            enemiesToSpawn = Mathf.RoundToInt((enemiesToSpawn + 1) * scalingMult);
            numRemaining = enemiesToSpawn;
        }
    }

    void SpawnEnemy()
    {
        if (Random.value < altChance)
            Instantiate(enemyPrefab[1], spawnPoints[Random.Range(0, spawnPoints.Length)].position, transform.rotation);
        else
            Instantiate(enemyPrefab[0], spawnPoints[Random.Range(0, spawnPoints.Length)].position, transform.rotation);
        
        enemiesActive++;

        StartCoroutine(StartCooldown());

        numRemaining--;
    }

    public void EnemyRemoved()
    {
        enemiesActive--;
    }

    public IEnumerator StartCooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(timeBtw);
        onCooldown = false;
    }
}
 