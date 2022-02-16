using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public enum AbilityType {
        passive,
        active,
        primary,
    }

    public new string name;
    public AbilityType type;
    public Sprite icon;
    public float[] cooldownTime;
    public float[] activeTime;
    public int maxLevel;

    public virtual void Activate(GameObject parent, int level) {}

    public virtual void BeginCooldown(GameObject parent, int level) {}
    
    public virtual void LevelUp() {} // Probbaly only used for passives
}
