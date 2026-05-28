using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerPerks : MonoBehaviour
{
    public int maxPerks = 10;
    private readonly HashSet<PerkType> perks = new();

    public event Action<PerkType> PerkAdded;
    public event Action<PerkType> PerkRemoved;
    public event Action PerksCleared;

    // Perk modifiers are computed on demand to avoid manual ApplyPerkModifiers calls
    public float SprintSpeedMultiplier => HasPerk(PerkType.SprintSpeed) ? 1.25f : 1f;
    public float ReloadSpeedMultiplier => HasPerk(PerkType.ReloadSpeed) ? 0.6f : 1f;
    public float FireRateMultiplier => HasPerk(PerkType.DoubleTap) ? 0.75f : 1f;
    public float ReviveSpeedMultiplier => HasPerk(PerkType.QuickRevive) ? 0.5f : 1f;
    public float HealthBonus => HasPerk(PerkType.HealthIncrease) ? 150f : 0f;
    public int MaxWeaponBonus => HasPerk(PerkType.ExtraWeaponSlot) ? 1 : 0;
    //woah
    public int MaxPerks => maxPerks;
    public IReadOnlyCollection<PerkType> OwnedPerks => perks;

    public bool TryAddPerk(PerkType perk)
    {
        if (perks.Count >= maxPerks)
        {
            return false;
        }

        if (!perks.Add(perk))
        {
            return false;
        }

        PerkAdded?.Invoke(perk);
        return true;
    }

    public bool RemovePerk(PerkType perk)
    {
        if (!perks.Remove(perk))
        {
            return false;
        }

        PerkRemoved?.Invoke(perk);
        return true;
    }

    public bool HasPerk(PerkType perk)
    {
        return perks.Contains(perk);
    }

    public void ClearPerks()
    {
        perks.Clear();
        PerksCleared?.Invoke();
    }
}
