using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AtrophyAura : Ability
{
    public int[] bonus;

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
        parent.GetComponent<Player>().damageBonus += bonus[level];
    }

    public void Reset(int round)
    {
        parent.GetComponent<Player>().damageBonus = 0;
    }

    public override void LevelUp()
    {
        if (level < maxLevel)
            level++;
    }
}
