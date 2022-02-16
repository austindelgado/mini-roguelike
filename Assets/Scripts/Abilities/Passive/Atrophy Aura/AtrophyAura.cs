using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AtrophyAura : Ability
{
    public int[] bonus;

    private int damageCount = 2; // Placeholder
    private GameObject parent;
    private int level;

    public override void Activate(GameObject parent, int level)
    {
        this.level = level;
        this.parent = parent;

        GameEvents.current.onEnemyDeath += DamageIncrease;
        GameEvents.current.onRoundEnd += Reset;
    }

    public void DamageIncrease(GameObject enemy)
    {
        damageCount += bonus[level];
        Debug.Log(parent.name + " damage increase by " + bonus[level] + ", total: " + damageCount);
    }

    public void Reset(int round)
    {
        damageCount = 2;
    }

    public override void LevelUp()
    {
        if (level < maxLevel)
            level++;
    }
}
