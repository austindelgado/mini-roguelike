using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    public AbilitySlot slot;
    public Image image;

    public void Setup(AbilitySlot slot)
    {
        this.slot = slot;

        GameEvents.current.onAbilityAdd += Refresh;

        if (slot.ability != null)
            image.sprite = slot.ability.icon;
    }

    public void Refresh(AbilitySlot slot)
    {
        if (slot == this.slot && slot.ability != null)
            image.sprite = slot.ability.icon;
    }
}
