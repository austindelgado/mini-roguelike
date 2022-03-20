using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class GridCell : MonoBehaviour
{
    private GameObject player = null;

    [SerializeField] private GameObject enemyPrefab = null;

    private List<GameObject> enemies = new List<GameObject>();
    private bool roundActive = false;

    private GameObject host;
    private GameObject challenger;

    private void Awake() => RoundSystem.AddGridCell(this);

    private void OnDestroy() => RoundSystem.RemoveGridCell(this);

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
        // If we're challenging, don't tp player or spawn enemies
        if(challenger == player)
            return;

        roundActive = true;

        // If hosting adjust teleport and teleport in other player
        if (host == player)
        {
            this.host = host;
            this.challenger = challenger;

            // Teleport our player on the left
            player.GetComponent<Player>().Teleport(transform.position + new Vector3(-3f, 0f, 0f), transform.position);
            
            // Teleport challenger on the right
            challenger.GetComponent<Player>().Teleport(transform.position + new Vector3(3f, 0f, 0f), transform.position);

            return;
        }

        // Teleport player in
        player.GetComponent<Player>().Teleport(transform.position, transform.position);

        // Start spawning enemies
        SpawnEnemy(round);
    }

    // Round ends when a player dies or all enemies are killed
    [Server]
    public void RoundEnd(bool win)
    {
        roundActive = false;

        player.GetComponent<Player>().RpcTeleportSpawn();
        GameEvents.current.PlayerRoundEnd(player.GetComponent<NetworkIdentity>().connectionToClient, win);

        ClearEnemies();
    }

    [Server]
    public void DuelEnd(GameObject winner, GameObject loser)
    {
        roundActive = false;

        winner.GetComponent<Player>().RpcTeleportSpawn();
        loser.GetComponent<Player>().RpcTeleportSpawn();
        GameEvents.current.DuelEnd(winner.GetComponent<NetworkIdentity>().connectionToClient, loser.GetComponent<NetworkIdentity>().connectionToClient);

        host = null;
        challenger = null;
    }

    [Server]
    private void SpawnEnemy(int round)
    {
        // for (int i = 0; i < round; i++)
        // {
            GameObject enemyInstance = Instantiate(enemyPrefab, transform.position + new Vector3(0f, 2f, 0f), transform.rotation); // TODO enemies should be children of grid but grid is currently scaled up from 1
            enemyInstance.GetComponent<Enemy>().SetGridPlayer(player, this);
            NetworkServer.Spawn(enemyInstance);
            enemies.Add(enemyInstance);
        // }
    }

    [Server]
    private void ClearEnemies()
    {
        if (enemies.Any())
        {
            foreach (var enemy in enemies)
                Destroy(enemy);
        }
    }

    [Server]
    public void EnemyDeath(GameObject enemy)
    {
        enemies.Remove(enemy);

        if (roundActive && !enemies.Any())
            RoundEnd(true);
    }

    [Server]
    public void PlayerDeath(GameObject player)
    {
        if (!roundActive)
            return;

        if (this.player == host && player == host) // Host loses
            DuelEnd(challenger, host);
        else if (this.player == host && player == challenger) // Host wins
            DuelEnd(host, challenger);
        else if (this.player == player) // Player died in PvE
            RoundEnd(false);
    }
}