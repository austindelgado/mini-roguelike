using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Weapon", menuName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    public enum FireType {single, burst, auto};

    [Header("Info")]
    [SerializeField] string id;
    public string ID { get {return id; } }
    public new string name;
    public string description;
    public Sprite sprite;
    public int price;

    [Header("Shooting")]
    public int damage; // Damage per bullet
    public int numBullet; // Number of bullets per shot
    public FireType type;
    public float speed; // base speed for projectiles
    public float spreadAngle; // Angle offset for each shot 
    public float fixedAngle; // Fixed offset for each shot
    public float burstDelay; // Only use on burst weapons for delay between shots

    [Header("Reloading")]
    public float fireDelay; // Time between shots
    public int ammoAmount; // Default max ammo
    public int ammoCost; // Ammo used per shot
    public float reloadTime; // Time for reload on empty magazine

    #if UNITY_EDITOR
    private void OnValidate()
    {
        string path = AssetDatabase.GetAssetPath(this);
        id = AssetDatabase.AssetPathToGUID(path);
    }
    #endif
}
