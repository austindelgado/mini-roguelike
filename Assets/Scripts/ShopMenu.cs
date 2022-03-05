using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopMenu : MonoBehaviour
{
    public Transform container;
    public Transform shopAbilityTemplate;

    public GameObject shopUI;
    public GameObject abilitySelectText;
    public GameObject inGameUI;

    public GameObject player;
    public AbilityDB abilityDB;
    public AbilityDB currentList;

    private int numAbilties;
    private int round = 1;

    public int levelPoints = 0; // Move to player at some point

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        inGameUI.SetActive(false);

        currentList = Instantiate(abilityDB); // Probably don't do this

        GameEvents.current.onRoundEnd += RoundEnd;

        // Get x number of primary options
        for (int i = 0; i < 3; i++)
        {
            CreateAbilityShopButton(currentList.GetPrimary());
        }
    }

    void GenerateChoices(int choices)
    {
        for (int i = 0; i < choices; i++)
        {
            CreateAbilityShopButton(currentList.GetAbility());
        }
    }

    private void CreateAbilityShopButton(Ability ability)
    {
        if (ability == null)
            return;

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
        
        abilitySelectText.SetActive(false);
    }

    public void AbilitySelect(Ability ability)
    {
        Component[] components = player.GetComponents(typeof(AbilitySlot));
        foreach(AbilitySlot component in components) {
            if (component.ability == null)
            {
                component.SetAbility(ability);
                break;
            }
        }

        currentList = Instantiate(abilityDB);
        ClearButtons();
    }

    public void Play()
    {
        player.GetComponent<AbilitySlot>().available = true;
        ToggleUI();
        Time.timeScale = 1.0f;
        //GameEvents.current.RoundStart(round);
    }

    public void ToggleUI()
    {
        shopUI.SetActive(!shopUI.activeSelf);
        inGameUI.SetActive(!inGameUI.activeSelf);
    }

    public void RoundEnd(int round)
    {
        this.round = round;
        ToggleUI();

        levelPoints++;

        if (round == 1)
        {
            abilitySelectText.SetActive(true);
            abilitySelectText.GetComponent<TextMeshProUGUI>().text = "Select Slot 2 Ability";
            GenerateChoices(4);
        }
        if (round == 2)
        {
            abilitySelectText.SetActive(true);
            abilitySelectText.GetComponent<TextMeshProUGUI>().text = "Select Slot 3 Ability";
            GenerateChoices(4);
        }
        if (round == 3)
        {
            abilitySelectText.SetActive(true);
            abilitySelectText.GetComponent<TextMeshProUGUI>().text = "Select Slot 4 Ability";
            GenerateChoices(4);
        }
    }
}
