using System;
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
}
