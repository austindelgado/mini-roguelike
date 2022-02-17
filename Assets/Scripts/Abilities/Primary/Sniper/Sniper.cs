using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Sniper : Ability
{
    public GameObject projectilePrefab;
    public int[] damage;

    public override void Activate(GameObject parent, int level)
    {
        GameObject projectile = Instantiate(projectilePrefab, parent.transform.position, parent.transform.rotation);
        projectile.GetComponent<Projectile>().parent = parent;
        projectile.GetComponent<Projectile>().dir = parent.GetComponent<Entity>().lookDir;
        projectile.GetComponent<Projectile>().damage = damage[level]  + parent.GetComponent<Player>().damageBonus;
    }
}
