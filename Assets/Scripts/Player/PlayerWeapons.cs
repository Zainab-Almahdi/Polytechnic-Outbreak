using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] private int baseMaxWeapons = 2;
    [SerializeField] private int equippedWeaponIndex;

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

    public bool TryAddWeapon(WeaponData weaponData)
    {
        if (weaponData == null || ownedWeapons.Count >= MaxWeaponsOwned)
        {
            return false;
        }

        ownedWeapons.Add(new WeaponInstance(weaponData));
        if (ownedWeapons.Count == 1)
        {
            equippedWeaponIndex = 0;
        }

        NotifyAmmoChanged();

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

        equippedWeaponIndex = index;
        NotifyAmmoChanged();
        return true;
    }

    public void EquipNextWeapon()
    {
        if (ownedWeapons.Count == 0)
        {
            return;
        }

        equippedWeaponIndex = (equippedWeaponIndex + 1) % ownedWeapons.Count;
        NotifyAmmoChanged();
    }

    public void EquipPreviousWeapon()
    {
        if (ownedWeapons.Count == 0)
        {
            return;
        }

        equippedWeaponIndex = (equippedWeaponIndex - 1 + ownedWeapons.Count) % ownedWeapons.Count;
        NotifyAmmoChanged();
    }

    public float GetReloadSpeedMultiplier()
    {
        return perks != null ? perks.ReloadSpeedMultiplier : 1f;
    }

    public bool RefillEquippedWeaponAmmo()
    {
        var weapon = GetEquippedWeapon();
        if (weapon == null || weapon.Data == null)
        {
            return false;
        }

        weapon.CurrentMagazineAmmo = weapon.Data.MagazineSize;
        weapon.CurrentReserveAmmo = weapon.Data.ReserveAmmo;
        NotifyAmmoChanged();
        return true;
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
}
