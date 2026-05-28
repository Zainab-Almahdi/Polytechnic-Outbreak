using System;
using System.Collections;
using UnityEngine;
using Assets.UI.Scripts;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float baseMaxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Regeneration")]
    [SerializeField] private float regenRate = 20f;
    [SerializeField] private float regenDelay = 5f;
    private float lastDamageTime;

    private PlayerPerks perks;
    private PlayerWeapons weapons;

    // Max health dynamically reflects perk modifiers without manual apply calls.
    public float MaxHealth => baseMaxHealth + (perks != null ? perks.HealthBonus : 0f);
    public float CurrentHealth => currentHealth;

    public event Action<float, float> HealthChanged;
    public event Action Died;

    private void Awake()
    {
        perks = GetComponent<PlayerPerks>();
        weapons = GetComponent<PlayerWeapons>();
    }

    private void Start()
    {
        currentHealth = MaxHealth;
        HealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    private void Update()
    {
        if (currentHealth < MaxHealth && Time.time - lastDamageTime >= regenDelay)
        {
            Heal(regenRate * Time.deltaTime);
        }
    }

    public void Damage(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        lastDamageTime = Time.time;
        SetCurrentHealth(currentHealth - amount);
    }

    public void Heal(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        SetCurrentHealth(currentHealth + amount);
    }

    private void SetCurrentHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0f, MaxHealth);
        HealthChanged?.Invoke(currentHealth, MaxHealth);
        if (currentHealth <= 0f)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        if (perks != null && perks.HasPerk(PerkType.QuickRevive))
        {
            StartCoroutine(ReviveSequence());
            return;
        }

        perks?.ClearPerks();
        Died?.Invoke();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDied();
        }
    }

    private IEnumerator ReviveSequence()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RecordDown();
        }

        // Clear perks
        if (perks != null)
        {
            perks.ClearPerks();
        }

        // Prune weapons (Mule Kick removal)
        if (weapons != null)
        {
            weapons.PruneWeapons();
        }

        // Restore health (now 100 because perks are gone)
        currentHealth = MaxHealth;
        HealthChanged?.Invoke(currentHealth, MaxHealth);

        // HUD message
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.SetInteractText("You have been revived by Quick Revive Perk", true);
            yield return new WaitForSeconds(3f);
            HUDManager.Instance.SetInteractText("", false);
        }
    }
}
