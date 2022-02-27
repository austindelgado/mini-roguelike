using System.Collections;
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

    public void RoundStart(int round)
    {
        // Need to check if our player is getting pulled into a duel
        roundActive = true;

        // Teleport player in
        player.GetComponent<Player>().Teleport(transform.position);

        // Start spawning enemies
        SpawnEnemy();
    }

    // Round ends when a player dies or all enemies are killed
    public void RoundEnd()
    {
        roundActive = false;

        player.GetComponent<Player>().TeleportSpawn();
        GameEvents.current.PlayerRoundEnd(player.GetComponent<NetworkIdentity>().connectionToClient);
    }

    [Server]
    private void SpawnEnemy()
    {
        enemyInstance = Instantiate(enemyPrefab, transform.position + new Vector3(0f, 2f, 0f), transform.rotation);
        NetworkServer.Spawn(enemyInstance);
    }

    public void PlayerDeath(GameObject player)
    {
        if (this.player == player && roundActive)
            RoundEnd();
    }
}