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

    [SyncVar(hook = nameof(OnGoldChanged))]
    public int gold = 100;
    [SerializeField]private GameObject goldUI = null;
    [SerializeField]private TMP_Text goldText = null;

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

    public override void OnStartAuthority()
    {
        goldUI.SetActive(true);
    }

    void OnGoldChanged(int _Old, int _New)
    {
        gold = _New;
        goldText.text = gold.ToString();
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
        goldText.text = gold.ToString();
    }
    
    [Server]
    public void ChangeGold(int goldAmount)
    {
        this.gold += goldAmount;
    }

    [Command]
    public void CmdPurchase(string ID)
    {
        WeaponData weaponData = Data.Instance.GetWeaponData(ID);

        if (gold >= weaponData.price)
        {
            gold -= weaponData.price;
            player.GetComponent<Weapon>().ServerEquip(ID);
        }
    }

    [Command]
    public void CmdBet(int amount, bool isHost)
    {
        if (gold < amount)
            return;

        GameEvents.current.BetPlaced(connectionToClient, amount, isHost);
        gold -= amount;
    }
}
