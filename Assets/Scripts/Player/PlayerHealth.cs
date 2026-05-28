using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float baseMaxHealth = 100f;
    [SerializeField] private float currentHealth;

    private PlayerPerks perks;

    // Max health dynamically reflects perk modifiers without manual apply calls.
    public float MaxHealth => baseMaxHealth + (perks != null ? perks.HealthBonus : 0f);
    public float CurrentHealth => currentHealth;

    public event Action<float, float> HealthChanged;
    public event Action Died;

    private void Awake()
    {
        perks = GetComponent<PlayerPerks>();
    }

    private void Start()
    {
        currentHealth = MaxHealth;
        HealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    public void Damage(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

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
        perks?.ClearPerks();
        Died?.Invoke();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDied();
        }
    }
}
