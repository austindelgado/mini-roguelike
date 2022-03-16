using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapon : NetworkBehaviour
{
    WeaponData weaponData;
    public int currentAmmo;

    public Projectile projectilePrefab; // Here because all are shared for now

    private void Fire()
    {
        if (!isServer)
        {
            Projectile projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            projectile.Initialize(gameObject, 0f);
        }

        CmdFire(gameObject, transform.position, transform.rotation, NetworkTime.time);
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
