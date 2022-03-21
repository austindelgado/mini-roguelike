using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class NetworkGamePlayerLobby : NetworkBehaviour
{
    [SyncVar] public string displayName = "Loading...";
    [SyncVar] public GameObject player = null;

    [SyncVar] public int gold = 100;

    private NetworkManagerLobby room;

    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null)
                return room;

            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
            
        Room.GamePlayers.Add(this);
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }
    
    [Server]
    public void ChangeGold(int goldAmount)
    {
        this.gold += goldAmount;
    }
}
