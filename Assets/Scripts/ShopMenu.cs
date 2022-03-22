using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class ShopMenu : MonoBehaviour
{
    [SerializeField] Button[] buttons;

    private WeaponData[] weapons;

    void Start()
    {
        // Fill buttons
        weapons = Data.Instance.GetAllWeapons();
    }

    public void Purchase(int index)
    {
        NetworkGamePlayerLobby localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<NetworkGamePlayerLobby>(); // A little clunky but gets local player
        localPlayer.CmdPurchase(weapons[index].ID);
    }
}
