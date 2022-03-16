using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapon : NetworkBehaviour
{
    public Transform weaponTransform;
    public WeaponData weaponData;
    public int currentAmmo;

    public Projectile projectilePrefab; // Here because all are shared for now

    // Player is currently responsible for rotating weaponTransform, maybe move that here in the future

    public void Fire()
    {
        if (!isServer)
        {
            Projectile projectile = Instantiate(projectilePrefab, weaponTransform.position, weaponTransform.rotation);
            projectile.Initialize(gameObject, 0f);
        }

        CmdFire(gameObject, weaponTransform.position, weaponTransform.rotation, NetworkTime.time);
    }

    [Command]
    void CmdFire(GameObject parent, Vector3 position, Quaternion rotation, double networkTime)
    {
        double timePassed = NetworkTime.time - networkTime;

        Projectile projectile = Instantiate(projectilePrefab, position, rotation);
        projectile.Initialize(parent, (float)timePassed); // Add passing in parent here

        RpcFire(parent, position, rotation, networkTime);
    }

    [ClientRpc]
    void RpcFire(GameObject parent, Vector3 position, Quaternion rotation, double networkTime)
    {
        if (hasAuthority)
            return;

        double timePassed = NetworkTime.time - networkTime;

        Projectile projectile = Instantiate(projectilePrefab, position, rotation);
        projectile.Initialize(parent, (float)timePassed); // Add passing in parent here
    }
}
