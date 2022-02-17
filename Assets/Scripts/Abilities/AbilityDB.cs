using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AbilityDB : ScriptableObject
{
    public List<Ability> primaries;
    public List<Ability> abilities;

    public Ability GetPrimary()
    {
        if (primaries.Count == 0)
            return null;

        int index = Random.Range(0, primaries.Count);
        Ability pick = primaries[index];
        primaries.RemoveAt(index);
        return pick;
    }

    public Ability GetAbility()
    {
        if (abilities.Count == 0)
            return null;

        int index = Random.Range(0, abilities.Count);
        Ability pick = abilities[index];
        abilities.RemoveAt(index);
        return pick;
    }
}
