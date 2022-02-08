using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySlot : MonoBehaviour
{
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

    public KeyCode key;

    // Update is called once per frame
    void Update()
    {
        // Level 1 check
        if (!available)
            return;

        switch (state)
        {
            case AbilityState.ready:
                if (Input.GetKey(key))
                {
                    ability.Activate(gameObject, level);
                    state = AbilityState.active;
                    activeTime = ability.activeTime[level];
                }
            break;
            case AbilityState.active:
                if (activeTime > 0)
                    activeTime -= Time.deltaTime;
                else
                {
                    ability.BeginCooldown(gameObject, level);
                    state = AbilityState.cooldown; 
                    cooldownTime = ability.cooldownTime[level];
                }
            break;
            case AbilityState.cooldown:
                if (cooldownTime > 0)
                    cooldownTime -= Time.deltaTime;
                else
                {
                    state = AbilityState.ready; 
                }
            break;
        }
    }
}
