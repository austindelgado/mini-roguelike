using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Bedlam : Ability
{
    public GameObject wispPrefab;
    private GameObject wisp;
    public int[] damage;

    public override void Activate(GameObject parent, int level)
    {
        wisp = Instantiate(wispPrefab, parent.transform.position + new Vector3(0, .5f, 0), parent.transform.rotation, parent.transform);
        wisp.GetComponent<Wisp>().parent = parent;
        wisp.GetComponent<Wisp>().damage = damage[level];
    }

    public override void BeginCooldown(GameObject parent, int level)
    {
        Destroy(wisp);
    }
}
