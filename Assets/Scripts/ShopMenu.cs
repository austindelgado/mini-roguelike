﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopMenu : MonoBehaviour
{
    public Transform container;
    public Transform shopAbilityTemplate;

    public GameObject shopUI;
    public GameObject inGameUI;

    public GameObject player;
    public Ability pistol;
    public Ability shotgun;
    public Ability sniper;

    private int numAbilties;
    private int round;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        inGameUI.SetActive(false);

        GameEvents.current.onRoundEnd += ToggleUI;
    }

    void OnEnable()
    {
        CreateAbilityShopButton(pistol);
        CreateAbilityShopButton(shotgun);
        CreateAbilityShopButton(sniper);
    }

    void GenerateChoices(int num)
    {

    }

    private void CreateAbilityShopButton(Ability ability)
    {
        // Place in right position
        Transform shopAbilityTransform = Instantiate(shopAbilityTemplate, container);
        RectTransform shopAbilityRectTransform = shopAbilityTransform.GetComponent<RectTransform>();

        // Fill out with ability data
        shopAbilityTransform.Find("abilityButton").GetComponent<Image>().sprite = ability.icon;
        shopAbilityTransform.Find("abilityButton").GetComponent<Button>().onClick.AddListener(delegate {AbilitySelect(ability);});
        shopAbilityTransform.GetChild(0).Find("abilityText").GetComponent<TextMeshProUGUI>().text = ability.name;

        numAbilties++;
    }

    private void ClearButtons()
    {
        foreach(Transform child in container)
            Destroy(child.gameObject);
    }

    public void AbilitySelect(Ability ability)
    {
        player.GetComponent<AbilitySlot>().ability = ability;
    }

    public void Play()
    {
        player.GetComponent<AbilitySlot>().available = true;
        ToggleUI(round);
        Time.timeScale = 1.0f;
    }

    public void ToggleUI(int round)
    {
        shopUI.SetActive(!shopUI.activeSelf);
        inGameUI.SetActive(!inGameUI.activeSelf);
    }
}
