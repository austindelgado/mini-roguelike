using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;

public class RoundSystem : NetworkBehaviour
{
    [SerializeField] private double timeBtwRound = 0;

    [SerializeField] private TMP_Text timerText = null;
    [SerializeField] private Image timerImage = null;
    [SerializeField] private GameObject timerObject = null;

    [SerializeField] private GameObject duelUI = null;
    [SerializeField] private TMP_Text hostUIText = null;
    [SerializeField] private TMP_Text challengerUIText = null;
    [SerializeField] private GameObject shopUI = null;
    [SerializeField] private GameObject betUI = null;
    
    [SerializeField] private ChatBehaviour roundChat = null;

    private int activeRounds;
    private int numRounds;
    private int roundNumber = 1;

    private double countdownStartTime;
    private bool countdownActive = false;
    private double timerStartTime;
    private bool timerActive = false;

    private NetworkGamePlayerLobby host;
    private NetworkGamePlayerLobby challenger;
    private List<Bet> hostBets = new List<Bet>();
    private List<Bet> challengerBets = new List<Bet>();

    private static List<GridCell> gridCells = new List<GridCell>();

    public static void AddGridCell(GridCell gridCell)
    {
        gridCells.Add(gridCell);

        gridCells = gridCells.OrderBy(x => x.transform.GetSiblingIndex()).ToList();
    }

    public static void RemoveGridCell(GridCell gridCell) => gridCells.Remove(gridCell);

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

    void Update()
    {
        if (countdownActive)
        {
            timerObject.SetActive(true);
            shopUI.SetActive(true);
            timerText.text = ((int)(timeBtwRound - (NetworkTime.time - countdownStartTime))).ToString();
            timerImage.fillAmount = (float)((timeBtwRound - (NetworkTime.time - countdownStartTime)) / timeBtwRound);
            
            if (isServer && timeBtwRound - (NetworkTime.time - countdownStartTime) < 0)
                StartRound();
        }
        else if (timerActive)
        {
            timerObject.SetActive(false);
            timerText.text = ((int)(NetworkTime.time - timerStartTime)).ToString();
        }
    }

    public void CountdownEnded()
    {

    }

    public override void OnStartServer()
    {
        NetworkManagerLobby.OnServerStopped += CleanUpServer;
        NetworkManagerLobby.OnServerReadied += CheckToStartRound;

        GameEvents.current.onDuelEnd += DuelEnd;
        GameEvents.current.onPlayerRoundEnd += PlayerEndRound;
        GameEvents.current.onBetPlaced += BetPlaced;
    }

    [ServerCallback]
    private void OnDestroy() => CleanUpServer();

    [Server]
    public void CleanUpServer()
    {
        NetworkManagerLobby.OnServerStopped -= CleanUpServer;
        NetworkManagerLobby.OnServerReadied -= CheckToStartRound;
    }

    [Server]
    public void AssignPlayers()
    {
        int nextIndex = 0;
        for (int i = Room.GamePlayers.Count - 1; i >= 0; i--)
        {
            GridCell gridCell = gridCells.ElementAtOrDefault(nextIndex);

            if (gridCell == null)
            {
                Debug.LogError($"Missing grid cell for player {nextIndex}");
                return;
            }

            gridCell.AssignPlayer(Room.GamePlayers[i].player);

            nextIndex++;
        }
    }

    [Server]
    public void StartRound()
    {
        if (host && challenger)
            GameEvents.current.RoundStart(roundNumber, host.player, challenger.player);
        else
            GameEvents.current.RoundStart(roundNumber, null, null);

        roundChat.ServerClear();
        roundChat.ServerSend("Round " + roundNumber +" Start!");
        RpcStartRound(NetworkTime.time);
    }

    [Server]
    private void CheckToStartRound(NetworkConnection conn)
    {
        if (Room.GamePlayers.Count(x => x.connectionToClient.isReady) != Room.GamePlayers.Count)
            return;
        
        AssignPlayers();

        if (isServer)
            PrepDuel();

        RpcStartCountdown(NetworkTime.time, 0, host, challenger);
    }

    [ClientRpc]
    private void RpcStartCountdown(double time, int round, NetworkGamePlayerLobby host, NetworkGamePlayerLobby challenger)
    {
        countdownStartTime = time;
        countdownActive = true;

        if (host != null && challenger != null)
            SetDuelIU(host, challenger);
    }

    [ClientRpc]
    private void RpcStartRound(double time)
    {
        countdownActive = false;

        timerActive = true;
        timerStartTime = time;

        activeRounds = Room.GamePlayers.Count;

        duelUI.SetActive(false);
        shopUI.SetActive(false);

        // Reset health
        for (int i = 0; i < Room.GamePlayers.Count; i++)
        {
            Room.GamePlayers[i].player.GetComponent<Health>().ResetHealth();
        }
    }

    [TargetRpc]
    public void EndRound()
    {
        timerActive = false;
    }

    [Server]
    public void PlayerEndRound(NetworkConnection target, bool win)
    {
        activeRounds--;

        // Get text of position finished
        int place = Room.GamePlayers.Count - activeRounds; // Get number of place finish
        string textPlace;
        switch (place % 10)
        {
            case 1: textPlace = place + "st"; break;
            case 2: textPlace = place + "nd"; break;
            case 3: textPlace = place + "rd"; break;
            default: textPlace = place + "th"; break;
        }

        int baseGold = 100 + 10 * (roundNumber - 1); // TODO Have this scale later
        int goldEarned = (int)Math.Floor(baseGold * (double)(8 - (place - 1))/8);

        if (win)
            roundChat.ServerSend(target.identity.gameObject.GetComponent<NetworkGamePlayerLobby>().displayName + " finished " + textPlace +" and earned " + goldEarned + " gold!");
        else
            roundChat.ServerSend(target.identity.gameObject.GetComponent<NetworkGamePlayerLobby>().displayName + " died!");

        RpcPlayerRoundEnd(target);
        target.identity.gameObject.GetComponent<NetworkGamePlayerLobby>().ChangeGold(goldEarned); // Gold per round is 100 * (activeRounds + 1 / )
        
        if (activeRounds == 0)
        {
            roundNumber++;
            PrepDuel();
            RpcStartCountdown(NetworkTime.time, 0, host, challenger);
        }
    }

    [Server]
    public void DuelEnd(NetworkConnection winner, NetworkConnection loser)
    {
        activeRounds -= 2; // 2 rounds end when a duel ends

        roundChat.ServerSend(winner.identity.gameObject.GetComponent<NetworkGamePlayerLobby>().displayName + " defeated " + loser.identity.gameObject.GetComponent<NetworkGamePlayerLobby>().displayName + "!");

        // Calculate bet multiplier
        float betMultiplier = 1;

        // Payout bets
        if (winner.identity.gameObject.GetComponent<NetworkGamePlayerLobby>() == host)
        {
            foreach (Bet bet in hostBets)
            {
                int playerAmount = Mathf.RoundToInt(bet.amount * betMultiplier);
                bet.better.identity.gameObject.GetComponent<NetworkGamePlayerLobby>().ChangeGold(playerAmount);
                roundChat.ServerSend(bet.better.identity.gameObject.GetComponent<NetworkGamePlayerLobby>().displayName + " won bet and earned " + playerAmount + "!");
            }

            // Payout duel winner
        }
        else
        {
            foreach (Bet bet in challengerBets)
            {
                int playerAmount = Mathf.RoundToInt(bet.amount * betMultiplier);
                bet.better.identity.gameObject.GetComponent<NetworkGamePlayerLobby>().ChangeGold(playerAmount);
                roundChat.ServerSend(bet.better.identity.gameObject.GetComponent<NetworkGamePlayerLobby>().displayName + " won bet and earned " + playerAmount + "!");
            }
            
            // Payout duel winner

        }

        
        if (activeRounds == 0)
        {
            roundNumber++; // TODO Clean this up, round might increment twice?
            PrepDuel();
            RpcStartCountdown(NetworkTime.time, 0, host, challenger);
        }
    }

    [TargetRpc]
    public void RpcPlayerRoundEnd(NetworkConnection target)
    {
        timerActive = false;
    }

    [Server]
    public void PrepDuel()
    {
        // Check round
        numRounds = Room.GamePlayers.Count; 
        if (Room.GamePlayers.Count > 1 && roundNumber % 2 == 0) // Duel every other round if there are more than 2 people TODO fix numbering
        {
            // Loop through and give chance to grab a player
            int numNeeded = 2;
            for (int i = Room.GamePlayers.Count; i > 0; i--)
            {
                // Chance is based of numNeeded / numberLeft
                float chance = (float)numNeeded / (float)i;
                
                // Should weigh in rounds since last duel here
                if (UnityEngine.Random.value < chance)
                {
                    if (numNeeded == 2)
                        host = Room.GamePlayers[i-1];
                    else 
                        challenger = Room.GamePlayers[i-1];

                    numNeeded--;
                }
            }
        }
        else // Solo player 
        {
            host = null;
            challenger = null;
        }
    }

    private void SetDuelIU(NetworkGamePlayerLobby host, NetworkGamePlayerLobby challenger)
    {

        duelUI.SetActive(true);
        hostUIText.text = host.displayName;
        challengerUIText.text = challenger.displayName;

        if (NetworkClient.localPlayer.gameObject.GetComponent<NetworkGamePlayerLobby>() == host || NetworkClient.localPlayer.gameObject.GetComponent<NetworkGamePlayerLobby>() == challenger) // Hide bets if player is in duel
            betUI.SetActive(false);
        else
            betUI.SetActive(true);
    }

    public class Bet {
        public NetworkConnection better;
        public int amount;

        public Bet(NetworkConnection better, int amount)
        {
            this.better = better;
            this.amount = amount;
        }
    }

    [Server]
    public void BetPlaced(NetworkConnection better, int amount, bool onHost)
    {
        if (host == null || challenger == null)
            return;

        roundChat.ServerSend(better.identity.gameObject.GetComponent<NetworkGamePlayerLobby>().displayName + " bet " + amount + " on " + (onHost == true ? host.displayName : challenger.displayName));

        // Create the bet
        if (onHost)
            hostBets.Add(new Bet(better, amount));
        else
            challengerBets.Add(new Bet(better, amount));
    }
}
