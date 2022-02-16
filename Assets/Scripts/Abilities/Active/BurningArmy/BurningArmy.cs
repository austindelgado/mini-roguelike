using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BurningArmy : Ability
{
    public GameObject skeletonPrefab;
    public int[] numSpawn;
    public int[] damage;

    private List<GameObject> spawns = new List<GameObject>();

    public override void Activate(GameObject parent, int level)
    {
        for (int i = 0; i < numSpawn[level]; i++)
        {
            Vector3 spawnMod = new Vector3(0,0,0);
            if (i == 0)
                spawnMod = new Vector3(0.5f, 0, 0);
            else if (i == 1)
                spawnMod = new Vector3(-0.5f, 0, 0);
            else if (i == 2)
                spawnMod = new Vector3(0, -0.5f, 0);
            else if (i == 3)
                spawnMod = new Vector3(0, 0.5f, 0);

            GameObject spawn = Instantiate(skeletonPrefab, parent.transform.position + spawnMod, parent.transform.rotation);
            spawn.GetComponent<Skeleton>().parent = parent;
            spawn.GetComponent<Skeleton>().damage = damage[level];
            spawns.Add(spawn);
        }
    }

    public override void BeginCooldown(GameObject parent, int level)
    {
        foreach (GameObject spawn in spawns)
            Destroy(spawn);
    }
}
