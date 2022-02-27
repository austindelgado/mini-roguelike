using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GridCell : MonoBehaviour
{
    private void Awake() => RoundSystem.AddGridCell(this);

    private void OnDestroy() => RoundSystem.RemoveGridCell(this);

    [Server]
    public void AssignPlayer(GameObject player)
    {
        Debug.Log(gameObject.name + " is assigned " + player.name);
    }
}