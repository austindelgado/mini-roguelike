using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FireBall : Ability
{
    public GameObject projectilePrefab;
    public int[] damage;
    public float[] distance;
    public float[] scaling;

    public override void Activate(GameObject parent, int level)
    {
        GameObject projectile = Instantiate(projectilePrefab, parent.transform.position, parent.transform.rotation);
        projectile.GetComponent<FireBallProjectile>().parent = parent;
        projectile.GetComponent<FireBallProjectile>().dir = parent.GetComponent<Entity>().lookDir;
        projectile.GetComponent<FireBallProjectile>().damage = damage[level];
        projectile.GetComponent<FireBallProjectile>().maxDistance = distance[level];
        projectile.GetComponent<FireBallProjectile>().scaling = scaling[level];
    }
}
