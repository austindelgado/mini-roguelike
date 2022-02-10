using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySlot : MonoBehaviour
{
    public Entity entity;

    public Ability ability;
    public int level;
    public bool available;

    float cooldownTime;
    float activeTime;

    enum AbilityState {
        ready, 
        active,
        cooldown
    }

    AbilityState state = AbilityState.ready;

    void Start()
    {
        // Grab entity
        entity = gameObject.GetComponent<Entity>();
    }

    public void Update()
    {
        if (state == AbilityState.active)
        {
            if (activeTime > 0)
                activeTime -= Time.deltaTime;
            else
            {
                ability.BeginCooldown(gameObject, level);
                state = AbilityState.cooldown; 
                cooldownTime = ability.cooldownTime[level];
            }
        }
        else if (state == AbilityState.cooldown)
        {
            if (cooldownTime > 0)
                cooldownTime -= Time.deltaTime;
            else
            {
                state = AbilityState.ready; 
            }
        }
    }

    public void Trigger()
    {
        if (!available)
            return;

        if (state == AbilityState.ready && ability.type != Ability.AbilityType.passive)
        {
            ability.Activate(gameObject, level);
            state = AbilityState.active;
            activeTime = ability.activeTime[level];
        }
    }

    // This will only happen on player click in the future
    public void LevelUp()
    {
        // Don't change level value on initial skill
        if (!available)
        {
            available = true;
            return;
        }

        if (level < ability.maxLevel)
        {
            level++;
        }
        else
            level = ability.maxLevel;
    }
}
