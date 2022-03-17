﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapon : NetworkBehaviour
{
    public Transform weaponTransform;
    public WeaponData weaponData;
    public int currentAmmo;

    private bool canShoot = true;
    private bool firing = false;
    private bool requireLift = false; // Used to force mouse release on reload

    public Projectile projectilePrefab; // Here because all are shared for now

    // Player is currently responsible for rotating weaponTransform, maybe move that here in the future

    public override void OnStartAuthority()
    {
        // This will need to change when we equip weapons
        currentAmmo = weaponData.ammoAmount;
    }

    public void ToggleFire(bool firing)
    {
        // On key down
        if (firing && !this.firing) // We want to shoot and we're not already
        {
            this.firing = true;
            StartCoroutine(Fire());
        }
        else if (firing && this.firing) // We want to shoot and already are
        {
            if (weaponData.type == WeaponData.FireType.auto && !requireLift) // Only if it's auto
                StartCoroutine(Fire());
        }
        else if (!firing) // We don't want to shoot
        {
            this.firing = false;
            requireLift = false;
        }
    }

    public IEnumerator Fire()
    {
        if (canShoot)
        {
            canShoot = false;
            // Check ammo
            if (currentAmmo - weaponData.ammoCost >= 0)
            {
                StartCoroutine(StartFireDelay());
                for (int i = 0; i < weaponData.numBullet; i++)
                {
                    SpawnProjectile(weaponTransform.position, weaponTransform.rotation);
                    yield return new WaitForSeconds(weaponData.burstDelay);
                }
                currentAmmo -= weaponData.ammoCost;
            }
            else
                StartCoroutine(StartReload());
        }
    }

    public IEnumerator StartFireDelay()
    {
        yield return new WaitForSeconds(weaponData.fireDelay);
        canShoot = true;
    }

    public IEnumerator StartReload()
    {
        requireLift = true;
        yield return new WaitForSeconds(weaponData.reloadTime);
        currentAmmo = weaponData.ammoAmount;
        canShoot = true;
    }

    public void SpawnProjectile(Vector3 position, Quaternion rotation)
    {
        if (!isServer)
        {
            Projectile projectile = Instantiate(projectilePrefab, position, rotation);
            projectile.Initialize(gameObject, weaponData.damage, weaponData.speed, 0f);
        }

        CmdSpawnProjectile(gameObject, weaponData.damage, weaponData.speed, position, rotation, NetworkTime.time);
    }

    [Command]
    void CmdSpawnProjectile(GameObject parent, int damage, float speed, Vector3 position, Quaternion rotation, double networkTime)
    {
        double timePassed = NetworkTime.time - networkTime;

        Projectile projectile = Instantiate(projectilePrefab, position, rotation);
        projectile.Initialize(parent, damage, speed, (float)timePassed); // Add passing in parent here

        RpcSpawnProjectile(parent, damage, speed, position, rotation, networkTime);
    }

    [ClientRpc]
    void RpcSpawnProjectile(GameObject parent, int damage, float speed, Vector3 position, Quaternion rotation, double networkTime)
    {
        if (hasAuthority)
            return;

        double timePassed = NetworkTime.time - networkTime;

        Projectile projectile = Instantiate(projectilePrefab, position, rotation);
        projectile.Initialize(parent, damage, speed, (float)timePassed); // Add passing in parent here
    }
}
