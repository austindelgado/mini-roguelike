using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Shotgun : Ability
{
    public GameObject projectilePrefab;

    public float maxSpread;
    public int[] pellets;

    public override void Activate(GameObject parent, int level)
    {
        Vector2 shotDir = parent.GetComponent<Entity>().lookDir;
        for (int i = 0; i < pellets[level]; i++)
        {
            float shotAngle = Random.Range(-maxSpread, maxSpread);
            GameObject projectile = Instantiate(projectilePrefab, parent.transform.position, parent.transform.rotation);
            projectile.GetComponent<Projectile>().dir = new Vector2(shotDir.x * Mathf.Cos(shotAngle * Mathf.Deg2Rad) - shotDir.y * Mathf.Sin(shotAngle * Mathf.Deg2Rad), shotDir.x * Mathf.Sin(shotAngle * Mathf.Deg2Rad) + shotDir.y * Mathf.Cos(shotAngle * Mathf.Deg2Rad));
        }
    }
}
