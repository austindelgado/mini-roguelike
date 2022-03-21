using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapon : NetworkBehaviour
{
    public Transform weaponTransform;
    public WeaponData weaponData;
    [SyncVar] public string weaponID;
    public int currentAmmo;

    private bool canShoot = true;
    private bool firing = false;
    private bool requireLift = false; // Used to force mouse release on reload

    public Projectile projectilePrefab; // Here because all are shared for now

    public void Start()
    {
        if (!hasAuthority && !isServer)
            Equip(weaponID);
    }

    // Player is currently responsible for rotating weaponTransform, maybe move that here in the future
    public void Equip(string ID) // Change this to use ID
    {
        weaponID = ID;
        this.weaponData = Data.Instance.GetWeaponData(weaponID);
        weaponTransform.gameObject.GetComponent<SpriteRenderer>().sprite = weaponData.sprite;
        currentAmmo = weaponData.ammoAmount;

        if (hasAuthority)
            CmdEquip(ID);
    }

    [Server]
    public void ServerEquip(string ID)
    {
        weaponID = ID;
        weaponData = Data.Instance.GetWeaponData(weaponID);
        weaponTransform.gameObject.GetComponent<SpriteRenderer>().sprite = weaponData.sprite;

        RpcEquip(ID);
    }

    [Command]
    void CmdEquip(string ID)
    {
        RpcEquip(ID);
    }

    [ClientRpc]
    void RpcEquip(string ID)
    {
        if (hasAuthority)
            return;

        weaponID = ID;
        weaponData = Data.Instance.GetWeaponData(weaponID);
        weaponTransform.gameObject.GetComponent<SpriteRenderer>().sprite = weaponData.sprite;
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
            if (weaponData.type == WeaponData.FireType.auto && !requireLift)
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

            // Check there's enough ammo
            if (currentAmmo - weaponData.ammoCost >= 0)
            {
                StartCoroutine(StartFireDelay()); // Fire delay starts before burst delay

                float fixedOffset = 0;
                if (weaponData.fixedAngle != 0)
                    fixedOffset = -(weaponData.fixedAngle / 2 * (weaponData.numBullet - 1)); // This spits back the -max angle

                for (int i = 0; i < weaponData.numBullet; i++)
                {
                    // Calculate spread
                    Quaternion offset = Quaternion.AngleAxis(fixedOffset + Random.Range(-weaponData.spreadAngle, weaponData.spreadAngle), Vector3.forward);
                    
                    SpawnProjectile(weaponTransform.position, offset * weaponTransform.rotation);

                    yield return new WaitForSeconds(weaponData.burstDelay);

                    if (weaponData.fixedAngle != 0)
                        fixedOffset += weaponData.fixedAngle;
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

    #region SpawningProjectiles

    public void SpawnProjectile(Vector3 position, Quaternion rotation)
    {
        if (!isServer)
        {
            Projectile projectile = Instantiate(projectilePrefab, position, rotation);
            projectile.Initialize(gameObject, weaponData.damage, weaponData.speed, 0f);
            CmdSpawnProjectile(gameObject, weaponData.damage, weaponData.speed, position, rotation, NetworkTime.time);
        }
        else
            RpcSpawnProjectile(gameObject, weaponData.damage, weaponData.speed, position, rotation, NetworkTime.time);
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
        if (!isServer && hasAuthority)
            return;

        double timePassed = NetworkTime.time - networkTime;

        Projectile projectile = Instantiate(projectilePrefab, position, rotation);
        projectile.Initialize(parent, damage, speed, (float)timePassed); // Add passing in parent here
    }

    #endregion
}
