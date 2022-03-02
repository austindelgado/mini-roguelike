using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class RoundSystem : NetworkBehaviour
{
    [SerializeField] private double timeBtwRound = 0;

    [SerializeField] private TMP_Text timerText = null;
    [SerializeField] private Image timerImage = null;
    [SerializeField] private GameObject timerObject = null;
    
    private int activeRounds;

    private double countdownStartTime;
    private bool countdownActive = false;
    private double timerStartTime;
    private bool timerActive = false;

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

        GameEvents.current.onPlayerRoundEnd += PlayerEndRound;
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
        GameEvents.current.RoundStart(0);
        RpcStartRound(NetworkTime.time);
    }

    [Server]
    private void CheckToStartRound(NetworkConnection conn)
    {
        if (Room.GamePlayers.Count(x => x.connectionToClient.isReady) != Room.GamePlayers.Count)
            return;
        
        AssignPlayers();
        RpcStartCountdown(NetworkTime.time);
    }

    [ClientRpc]
    private void RpcStartCountdown(double time)
    {
        countdownStartTime = time;
        countdownActive = true;

        if (isServer)
            PrepDuel();
    }

    [ClientRpc]
    private void RpcStartRound(double time)
    {
        countdownActive = false;

        timerActive = true;
        timerStartTime = time;

        activeRounds = Room.GamePlayers.Count;

        Debug.Log("Start");
    }

    [TargetRpc]
    public void EndRound()
    {
        timerActive = false;
    }

    [Server]
    public void PlayerEndRound(NetworkConnection target)
    {
        activeRounds--;
        RpcPlayerRoundEnd(target);
        
        if (activeRounds == 0)
            RpcStartCountdown(NetworkTime.time);
    }

    [TargetRpc]
    public void RpcPlayerRoundEnd(NetworkConnection target)
    {
        Debug.Log("Player finished round");
        timerActive = false;
    }

    [Server]
    public void PrepDuel()
    {
        // Check round

        if (Room.GamePlayers.Count > 1)
        {
            // Loop through and give chance to grab a player
            int numNeeded = 2;
            for (int i = Room.GamePlayers.Count; i > 0; i--)
            {
                // Chance is based of numNeeded / numberLeft
                float chance = (float)numNeeded / (float)i;
                
                // Should weigh in rounds since last duel here
                if (Random.value < chance)
                {
                    numNeeded--;
                }
            }
        }
    }
}
