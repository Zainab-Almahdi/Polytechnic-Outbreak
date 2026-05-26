using UnityEngine;

// Static weapon data container; runtime state lives in WeaponInstance.
[CreateAssetMenu(menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string WeaponName;
    public int FireRateRpm;
    public int BurstFireRateRpm;
    public float Damage;
    public float HeadshotMultiplier = 2f;
    public float Range = 100f;
    public float ReloadSpeedSeconds;
    public int MagazineSize;
    public int ReserveAmmo;
    public bool IsBurstWeapon;
    public int BurstCount = 3;
    public bool IsShotgun;
    public int PelletCount = 8;
    public float SpreadAngle = 5f;
}
