using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Feast : Ability
{
    public float[] percentage;

    private GameObject parent;
    private int level;

    public override void Activate(GameObject parent, int level)
    {
        this.level = level;
        this.parent = parent;

        GameEvents.current.onDamageDealt += LifeSteal;
    }

    public void LifeSteal(int damage, GameObject dealer)
    {
        if (dealer == parent)
        {
            Debug.Log(parent.name + " feasted for " + damage + " * " + percentage[level]);
        }
    }

    public override void LevelUp()
    {
        if (level < maxLevel)
            level++;
    }
}
