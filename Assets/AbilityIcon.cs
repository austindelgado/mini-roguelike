using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    public AbilitySlot slot;
    public Image image;

    public void Update()
    {

    }

    public void Setup(AbilitySlot slot)
    {
        this.slot = slot;

        if (slot.ability != null)
            image.sprite = slot.ability.icon;
    }
}
