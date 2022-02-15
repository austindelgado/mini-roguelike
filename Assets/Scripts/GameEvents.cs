﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public event Action<int> onRoundStart;
    public void RoundStart(int round)
    {
        Debug.Log("Round " + round + " start!");
        if (onRoundStart != null)
            onRoundStart(round);
    }

    public event Action<int> onRoundEnd;
    public void RoundEnd(int round)
    {
        Debug.Log("Round " + round + " end!");
        if (onRoundEnd != null)
            onRoundEnd(round);
    }
}
