using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Pistol : Ability
{
    public GameObject projectilePrefab;

    public override void Activate(GameObject parent, int level)
    {
        GameObject projectile = Instantiate(projectilePrefab, parent.transform.position, parent.transform.rotation);
        projectile.GetComponent<Projectile>().dir = parent.GetComponent<CharacterController>().lookDir;
    }
}
