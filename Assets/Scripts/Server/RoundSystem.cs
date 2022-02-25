using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class RoundSystem : NetworkBehaviour
{
    [SerializeField] private Animator animator = null;

    private static List<Transform> gridPoints = new List<Transform>();

    private int nextIndex = 0;

    public static void AddGridPoint(Transform transform)
    {
        gridPoints.Add(transform);

        gridPoints = gridPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveGridPoint(Transform transform) => gridPoints.Remove(transform);

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

    public void CountdownEnded()
    {
        animator.enabled = false;
    }

    public override void OnStartServer()
    {
        NetworkManagerLobby.OnServerStopped += CleanUpServer;
        NetworkManagerLobby.OnServerReadied += CheckToStartRound;
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
    public void StartRound()
    {
        for (int i = Room.GamePlayers.Count - 1; i >= 0; i--)
        {
            Transform gridPoint = gridPoints.ElementAtOrDefault(nextIndex);

            if (gridPoint == null)
            {
                Debug.LogError($"Missing spawn point for player {nextIndex}");
                return;
            }

            Room.GamePlayers[i].player.GetComponent<Player>().Teleport(gridPoint.position);

            nextIndex++;
        }
        
        nextIndex = 0;

        RpcStartRound();
    }

    [Server]
    private void CheckToStartRound(NetworkConnection conn)
    {
        if (Room.GamePlayers.Count(x => x.connectionToClient.isReady) != Room.GamePlayers.Count)
            return;

        animator.enabled = true;
        
        //RpcStartCountdown();
        //StartRound();
    }

    [ClientRpc]
    private void RpcStartCountdown()
    {
        animator.enabled = true;
    }

    [ClientRpc]
    private void RpcStartRound()
    {
        Debug.Log("Start");
    }

    [Server]
    public void EndRound()
    {
        for (int i = Room.GamePlayers.Count - 1; i >= 0; i--)
        {
            Room.GamePlayers[i].player.GetComponent<Player>().TeleportSpawn();

            nextIndex++;
        }
        
        nextIndex = 0;

        RpcStartRound();
    }
}
