using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action<int, GameObject> onDamageDealt;
    public void DamageDealt(int damage, GameObject dealer)
    {
        if (onDamageDealt != null)
            onDamageDealt(damage, dealer);
    }

    public event Action<GameObject> onPlayerDeath;
    public void PlayerDeath(GameObject player)
    {
        if (onPlayerDeath != null)
            onPlayerDeath(player);
    }

    public event Action<GameObject> onEnemyDeath;
    public void EnemyDeath(GameObject enemy)
    {
        if (onEnemyDeath != null)
            onEnemyDeath(enemy);
    }

    public event Action<int, GameObject, GameObject> onRoundStart;
    public void RoundStart(int round, GameObject host, GameObject challenger)
    {
        if (challenger != null && host != null)
            Debug.Log("Round " + round + " start! " + challenger.name + " is challenging " + host.name);
        if (onRoundStart != null)
            onRoundStart(round, host, challenger);
    }

    public event Action<int> onRoundEnd;
    public void RoundEnd(int round)
    {
        Debug.Log("Round " + round + " end!");
        if (onRoundEnd != null)
            onRoundEnd(round);
    }

    public event Action<NetworkConnection, NetworkConnection> onDuelEnd;
    public void DuelEnd(NetworkConnection winner, NetworkConnection loser)
    {
        if (onDuelEnd != null)
            onDuelEnd(winner, loser);
    }

    public event Action<NetworkConnection, bool> onPlayerRoundEnd;
    public void PlayerRoundEnd(NetworkConnection connection, bool win)
    {
        if (onPlayerRoundEnd != null)
            onPlayerRoundEnd(connection, win);
    }

    public event Action<AbilitySlot> onAbilityAdd;
    public void AbilityAdd(AbilitySlot slot)
    {
        Debug.Log(slot.ability.name + " added!");
        if (onAbilityAdd != null)
            onAbilityAdd(slot);
    }

    public event Action<NetworkConnection, int, bool> onBetPlaced;
    public void BetPlaced(NetworkConnection connection, int amount, bool onHost)
    {
        if (onBetPlaced != null)
            onBetPlaced(connection, amount, onHost);
    }
}
