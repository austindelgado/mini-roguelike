using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class WeaponSelectDebug : MonoBehaviour
{
    public TextMeshProUGUI output;

    public List<WeaponData> weapons;

    public void HandleInputData(int val)
    {
        GameObject localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<NetworkGamePlayerLobby>().player; // A little clunky but gets local player

        localPlayer.GetComponent<Weapon>().Equip(weapons[val].ID);
    }
}
