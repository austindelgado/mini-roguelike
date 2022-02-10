using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
    public GameObject shopUI;
    public GameObject player;
    public Ability pistol;
    public Ability shotgun;
    public Ability sniper;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
    }

    public void AbilitySelect(int num)
    {
        if (num == 0)
            player.GetComponent<AbilitySlot>().ability = pistol;
        else if (num == 1)
            player.GetComponent<AbilitySlot>().ability = shotgun;
        else if (num == 2)
            player.GetComponent<AbilitySlot>().ability = sniper;
    }

    public void Play()
    {
        player.GetComponent<AbilitySlot>().available = true;

        shopUI.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
