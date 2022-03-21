using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class BetUI : NetworkBehaviour
{
    [SerializeField] private TMP_InputField hostBet = null;
    [SerializeField] private TMP_InputField challengerBet = null;

    [Client]
    public void Submit()
    {
        if (string.IsNullOrWhiteSpace(hostBet.text) && string.IsNullOrWhiteSpace(challengerBet.text)) // Both blank
            return;
        else if (!string.IsNullOrWhiteSpace(hostBet.text) && !string.IsNullOrWhiteSpace(challengerBet.text)) // Both filled
        {
            hostBet.text = string.Empty;
            challengerBet.text = string.Empty;
        }
        else // Only one filled
        {
            int amount;
            if (!string.IsNullOrWhiteSpace(hostBet.text))
            {
                amount = int.Parse(hostBet.text);
                Debug.Log(amount);
                NetworkClient.localPlayer.gameObject.GetComponent<NetworkGamePlayerLobby>().CmdBet(amount, true);
            }
            else
            {
                amount = int.Parse(challengerBet.text);
                Debug.Log(amount);
                NetworkClient.localPlayer.gameObject.GetComponent<NetworkGamePlayerLobby>().CmdBet(amount, false);
            }

            // On bet, hide inputs
            gameObject.SetActive(false);
        }
    }
}
