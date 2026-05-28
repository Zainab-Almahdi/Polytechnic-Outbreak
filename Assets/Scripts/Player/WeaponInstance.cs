using System;
using UnityEngine;

[Serializable]
public class WeaponInstance
{
    public WeaponData Data;
    public GameObject Prefab;
    public GameObject SpawnedObject;
    public string WeaponName;
    public int FireRateRpm;
    public int BurstFireRateRpm;
    public float Damage;
    public float HeadshotMultiplier;
    public float Range;
    public float ReloadSpeedSeconds;
    public int MagazineSize;
    public int ReserveAmmo;
    public bool IsBurstWeapon;
    public int BurstCount;
    public bool IsShotgun;
    public int PelletCount;
    public float SpreadAngle;
    public int CurrentMagazineAmmo;
    public int CurrentReserveAmmo;
    public int UpgradeLevel;

    public WeaponInstance(WeaponData data, GameObject prefab = null)
    {
        Prefab = prefab;
        ApplyData(data);
    }

    public WeaponInstance(Gun gun, GameObject prefab = null)
    {
        Prefab = prefab;
        ApplyGun(gun, prefab != null ? prefab.name : null);
    }

    private void ApplyData(WeaponData data)
    {
        Data = data;
        if (data == null)
        {
            return;
        }

        WeaponName = data.WeaponName;
        FireRateRpm = data.FireRateRpm;
        BurstFireRateRpm = data.BurstFireRateRpm;
        Damage = data.Damage;
        HeadshotMultiplier = data.HeadshotMultiplier;
        Range = data.Range;
        ReloadSpeedSeconds = data.ReloadSpeedSeconds;
        MagazineSize = data.MagazineSize;
        ReserveAmmo = data.ReserveAmmo;
        IsBurstWeapon = data.IsBurstWeapon;
        BurstCount = data.BurstCount;
        IsShotgun = data.IsShotgun;
        PelletCount = data.PelletCount;
        SpreadAngle = data.SpreadAngle;

        CurrentMagazineAmmo = MagazineSize;
        CurrentReserveAmmo = ReserveAmmo;
    }

    private void ApplyGun(Gun gun, string weaponName)
    {
        if (gun == null)
        {
            return;
        }

        string displayName = string.IsNullOrWhiteSpace(gun.displayName)
            ? null
            : gun.displayName;
        WeaponName = !string.IsNullOrWhiteSpace(displayName)
            ? displayName
            : string.IsNullOrWhiteSpace(weaponName)
                ? gun.name
                : weaponName;
        Damage = gun.damage;
        HeadshotMultiplier = gun.headshotMultiplier;
        Range = gun.range;
        ReloadSpeedSeconds = gun.reloadTime;
        MagazineSize = gun.magazineSize;
        ReserveAmmo = gun.reserveAmmo;
        IsBurstWeapon = gun.isBurstWeapon;
        BurstCount = gun.burstCount;
        IsShotgun = gun.isShotgun;
        PelletCount = gun.pelletCount;
        SpreadAngle = gun.spreadAngle;

        if (gun.fireRate > 0f)
        {
            FireRateRpm = Mathf.RoundToInt(60f / gun.fireRate);
            BurstFireRateRpm = FireRateRpm;
        }

        CurrentMagazineAmmo = MagazineSize;
        CurrentReserveAmmo = ReserveAmmo;
    }
}
