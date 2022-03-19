using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    // Singleton (ew) setup
    private static Data instance;
    public static Data Instance { get { return instance; } } // Use Data.Instance.

    public List<WeaponData> weaponDB; // This should be a dictionary but unity is not a fan, find a better way to look up in the future
    // Also need to find a way to auto populate from folder of weaponData

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    public WeaponData GetWeaponData(string ID)
    {
        foreach (var weaponData in weaponDB)
        {
            if (weaponData.ID == ID)
                return weaponData;
        }

        return null;
    }

    public WeaponData[] GetAllWeapons()
    {
        return weaponDB.ToArray();
    }
}
