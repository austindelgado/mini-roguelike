using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityIcon : MonoBehaviour
{
    public AbilitySlot slot;
    public Image image;
    public TextMeshProUGUI lvlText;
    public GameObject levelButton;

    public ShopMenu shop;

    public void Update()
    {
        if (shop.levelPoints == 0)
            levelButton.SetActive(false);
    }

    public void Setup(AbilitySlot slot)
    {
        this.slot = slot;

        GameEvents.current.onAbilityAdd += Refresh;
        GameEvents.current.onRoundEnd += LevelButton;

        // Find shop
        shop = GameObject.Find("InBetween").GetComponent<ShopMenu>();

        Refresh(slot);
    }

    public void Refresh(AbilitySlot slot)
    {
        if (slot == this.slot && slot.ability != null)
        {
            image.sprite = slot.ability.icon;

            if (slot.available)
                lvlText.text = "LVL: " + (slot.level + 1);
            else
                lvlText.text = "Unavailable";

            LevelButton(0);
        }
    }

    public void LevelButton(int round)
    {
        if (shop.levelPoints == 0 || slot.ability == null || slot.level == slot.ability.maxLevel)
        {
            levelButton.SetActive(false);
        }
        else
        {
            levelButton.SetActive(true);
        }
    }

    public void LevelUp()
    {
        if (shop.levelPoints > 0)
        {
            shop.levelPoints--;
            slot.LevelUp();
            Refresh(slot);
        }
    }
}
