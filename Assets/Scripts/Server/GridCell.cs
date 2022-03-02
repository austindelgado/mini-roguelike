﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GridCell : MonoBehaviour
{
    private GameObject player = null;

    [SerializeField] private GameObject enemyPrefab = null;

    private GameObject enemyInstance;
    private bool roundActive = false;

    private void Awake() => RoundSystem.AddGridCell(this);

    private void OnDestroy() => RoundSystem.RemoveGridCell(this);

    void Update()
    {
        if (roundActive && enemyInstance == null)
            RoundEnd();
    }

    [Server]
    public void AssignPlayer(GameObject player)
    {
        Debug.Log(gameObject.name + " is assigned " + player.name);
        this.player = player;

        // With a player assigned, we want to subscribe to RoundStart event
        GameEvents.current.onRoundStart += RoundStart;
        GameEvents.current.onPlayerDeath += PlayerDeath;
    }

    [Server]
    public void RoundStart(int round, GameObject host, GameObject challenger)
    {
        roundActive = true;

        // If we're challenging, don't tp player or spawn enemies
        if(challenger == player)
        {
            enemyInstance = host;
            return;
        }

        // If hosting adjust teleport and teleport in other player
        if (host == player)
        {
            // Teleport our player on the left
            player.GetComponent<Player>().Teleport(transform.position + new Vector3(-3f, 0f, 0f), transform.position);
            
            // Teleport challenger on the right
            challenger.GetComponent<Player>().Teleport(transform.position + new Vector3(3f, 0f, 0f), transform.position);

            enemyInstance = challenger;

            return;
        }

        // Teleport player in
        player.GetComponent<Player>().Teleport(transform.position, transform.position);

        // Start spawning enemies
        SpawnEnemy();
    }

    // Round ends when a player dies or all enemies are killed
    [Server]
    public void RoundEnd()
    {
        roundActive = false;

        Debug.Log(player.name + " ended round!");
        player.GetComponent<Player>().RpcTeleportSpawn();
        GameEvents.current.PlayerRoundEnd(player.GetComponent<NetworkIdentity>().connectionToClient);
    }

    [Server]
    private void SpawnEnemy()
    {
        enemyInstance = Instantiate(enemyPrefab, transform.position + new Vector3(0f, 2f, 0f), transform.rotation);
        NetworkServer.Spawn(enemyInstance);
    }

    [Server]
    public void PlayerDeath(GameObject player)
    {
        if (this.player == player && roundActive || enemyInstance == player)
            RoundEnd();
    }
}