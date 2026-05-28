using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] private int baseMaxWeapons = 2;
    [SerializeField] private int equippedWeaponIndex;
    [SerializeField] private WeaponData defaultWeapon;
    [SerializeField] private GameObject defaultWeaponPrefab;
    [SerializeField] private Transform weaponMount;

    private PlayerPerks perks;
    private readonly List<WeaponInstance> ownedWeapons = new();

    public event Action<int, int> AmmoChanged;

    // Weapon ownership uses runtime instances to avoid modifying shared data.
    public IReadOnlyList<WeaponInstance> OwnedWeapons => ownedWeapons;
    public int MaxWeaponsOwned => baseMaxWeapons + (perks != null ? perks.MaxWeaponBonus : 0);

    private void Awake()
    {
        perks = GetComponent<PlayerPerks>();
    }

    private void Start()
    {
        if (defaultWeaponPrefab != null && ownedWeapons.Count == 0)
        {
            TryAddWeapon(defaultWeaponPrefab);
        }
        else if (defaultWeapon != null && ownedWeapons.Count == 0)
        {
            TryAddWeapon(defaultWeapon);
        }
    }

    public bool TryAddWeapon(WeaponData weaponData)
    {
        if (weaponData == null)
        {
            return false;
        }

        return TryAddWeaponInternal(new WeaponInstance(weaponData));
    }

    public bool TryAddWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
        {
            return false;
        }

        var gun = weaponPrefab.GetComponent<Gun>();
        if (gun == null)
        {
            Debug.LogWarning("PlayerWeapons: weapon prefab is missing a Gun component.");
            return false;
        }

        return TryAddWeaponInternal(new WeaponInstance(gun, weaponPrefab));
    }

    private bool TryAddWeaponInternal(WeaponInstance weaponInstance)
    {
        if (weaponInstance == null || ownedWeapons.Count >= MaxWeaponsOwned)
        {
            return false;
        }

        ownedWeapons.Add(weaponInstance);
        EnsureSpawned(weaponInstance);
        
        // Equip the newly added weapon
        SetEquippedWeapon(ownedWeapons.Count - 1);

        return true;
    }

    public bool RemoveWeapon(WeaponInstance weapon)
    {
        if (weapon == null)
        {
            return false;
        }

        var removed = ownedWeapons.Remove(weapon);
        if (removed)
        {
            equippedWeaponIndex = Mathf.Clamp(equippedWeaponIndex, 0, ownedWeapons.Count - 1);
            NotifyAmmoChanged();
        }

        return removed;
    }

    public WeaponInstance GetEquippedWeapon()
    {
        if (ownedWeapons.Count == 0)
        {
            return null;
        }

        equippedWeaponIndex = Mathf.Clamp(equippedWeaponIndex, 0, ownedWeapons.Count - 1);
        return ownedWeapons[equippedWeaponIndex];
    }

    public bool TryEquipWeapon(int index)
    {
        if (index < 0 || index >= ownedWeapons.Count)
        {
            return false;
        }

        SetEquippedWeapon(index);
        return true;
    }

    public void EquipNextWeapon()
    {
        if (ownedWeapons.Count == 0)
        {
            return;
        }

        SetEquippedWeapon((equippedWeaponIndex + 1) % ownedWeapons.Count);
    }

    public void EquipPreviousWeapon()
    {
        if (ownedWeapons.Count == 0)
        {
            return;
        }

        SetEquippedWeapon((equippedWeaponIndex - 1 + ownedWeapons.Count) % ownedWeapons.Count);
    }

    public float GetReloadSpeedMultiplier()
    {
        return perks != null ? perks.ReloadSpeedMultiplier : 1f;
    }

    public float GetFireRateMultiplier()
    {
        return perks != null ? perks.FireRateMultiplier : 1f;
    }

    public bool RefillEquippedWeaponAmmo()
    {
        var weapon = GetEquippedWeapon();
        if (weapon == null)
        {
            return false;
        }

        weapon.CurrentMagazineAmmo = weapon.MagazineSize;
        weapon.CurrentReserveAmmo = weapon.ReserveAmmo;
        NotifyAmmoChanged();
        return true;
    }

    public void InvokeAmmoChanged(int currentMag, int reserve)
    {
        AmmoChanged?.Invoke(currentMag, reserve);
    }

    private void NotifyAmmoChanged()
    {
        var weapon = GetEquippedWeapon();
        if (weapon == null)
        {
            AmmoChanged?.Invoke(0, 0);
            return;
        }

        AmmoChanged?.Invoke(weapon.CurrentMagazineAmmo, weapon.CurrentReserveAmmo);
    }

    public bool HasWeapon(GameObject prefab)
    {
        if (prefab == null) return false;
        foreach (var weapon in ownedWeapons)
        {
            if (weapon.Prefab == prefab) return true;
        }
        return false;
    }

    public bool RefillWeaponAmmo(GameObject prefab)
    {
        foreach (var weapon in ownedWeapons)
        {
            if (weapon.Prefab == prefab)
            {
                weapon.CurrentMagazineAmmo = weapon.MagazineSize;
                weapon.CurrentReserveAmmo = weapon.ReserveAmmo;
                NotifyAmmoChanged();
                return true;
            }
        }
        return false;
    }

    private void SetEquippedWeapon(int index)
    {
        if (index < 0 || index >= ownedWeapons.Count)
        {
            return;
        }

        for (var i = 0; i < ownedWeapons.Count; i++)
        {
            var instance = ownedWeapons[i];
            if (instance == null)
            {
                continue;
            }

            EnsureSpawned(instance);
            if (instance.SpawnedObject != null)
            {
                instance.SpawnedObject.SetActive(i == index);
            }
        }

        equippedWeaponIndex = index;
        NotifyAmmoChanged();
    }

    private void EnsureSpawned(WeaponInstance weaponInstance)
    {
        if (weaponInstance == null || weaponInstance.SpawnedObject != null || weaponInstance.Prefab == null)
        {
            return;
        }

        var parent = weaponMount != null ? weaponMount : transform;
        weaponInstance.SpawnedObject = Instantiate(weaponInstance.Prefab, parent);
        weaponInstance.SpawnedObject.transform.localPosition = Vector3.zero;
        weaponInstance.SpawnedObject.transform.localRotation = Quaternion.identity;
        weaponInstance.SpawnedObject.transform.localScale = Vector3.one * 0.2f;
        weaponInstance.SpawnedObject.SetActive(false);

        var gun = weaponInstance.SpawnedObject.GetComponent<Gun>();
        if (gun != null)
        {
            gun.Initialize(weaponInstance, this);
        }
    }
}
