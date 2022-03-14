using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Used for tracking entity stats, goes on gameobject
public class Stats : NetworkBehaviour
{
    [SyncVar] private int maxHealth;
    [SyncVar] private int topSpeed;
}
