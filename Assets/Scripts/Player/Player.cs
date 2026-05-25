using System;
using Assets.UI.Scripts;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Main player coordinator: only caches subsystem references.
    public PlayerHealth Health { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerWeapons Weapons { get; private set; }
    public PlayerPerks Perks { get; private set; }
    public PlayerInventory Inventory { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerPowerups Powerups { get; private set; }
    public PlayerPoints Points { get; private set; }

    private void Awake()
    {
        Health = GetComponent<PlayerHealth>();
        Movement = GetComponent<PlayerMovement>();
        Weapons = GetComponent<PlayerWeapons>();
        Perks = GetComponent<PlayerPerks>();
        Inventory = GetComponent<PlayerInventory>();
        Interaction = GetComponent<PlayerInteraction>();
        Powerups = GetComponent<PlayerPowerups>();
        Points = GetComponent<PlayerPoints>();
    }

    private void Start()
    {
        if (Health != null)
        {
            Health.HealthChanged += OnHealthChanged;
            OnHealthChanged(Health.CurrentHealth, Health.MaxHealth);
        }

        if (Weapons != null)
        {
            Weapons.AmmoChanged += OnAmmoChanged;
            var weapon = Weapons.GetEquippedWeapon();
            if (weapon != null)
            {
                OnAmmoChanged(weapon.CurrentMagazineAmmo, weapon.CurrentReserveAmmo);
            }
        }

        if (Points != null)
        {
            Points.PointsChanged += OnPointsChanged;
            OnPointsChanged(Points.currentPoints);
        }
    }

    private void OnDestroy()
    {
        if (Health != null)
        {
            Health.HealthChanged -= OnHealthChanged;
        }

        if (Weapons != null)
        {
            Weapons.AmmoChanged -= OnAmmoChanged;
        }

        if (Points != null)
        {
            Points.PointsChanged -= OnPointsChanged;
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.SetHealthValue($"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}");
        }
    }

    private void OnAmmoChanged(int currentMag, int reserve)
    {
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.SetCurrentMagValue(currentMag.ToString());
            HUDManager.Instance.SetReserveMagValue(reserve.ToString());
        }
    }

    private void OnPointsChanged(int points)
    {
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.SetPlayer1MoneyLabel(points.ToString());
        }
    }
}
