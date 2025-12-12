using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;

    [Header("Shooting")]
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 0.15f;

    [Header("Ammo")]
    public int maxAmmo = 30;
    public int currentAmmo = 30;
    public int reserveAmmo = 90;

    [Header("Recoil")]
    public float recoilX = 2f;
    public float recoilY = 1f;
}
