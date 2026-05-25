using UnityEngine;

// Static weapon data container; runtime state lives in WeaponInstance.
[CreateAssetMenu(menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string WeaponName;
    public int FireRateRpm;
    public int BurstFireRateRpm;
    public float Damage;
    public float ReloadSpeedSeconds;
    public int MagazineSize;
    public int ReserveAmmo;
}
