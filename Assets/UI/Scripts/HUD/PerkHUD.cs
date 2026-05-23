using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Scripts
{
    public class PerkHUD : MonoBehaviour
    {
        [Serializable]
        private struct PerkIconEntry
        {
            public PerkType Perk;
            public Sprite Icon;
        }

        [Header("Slots")]
        [SerializeField] private List<Image> perkSlots = new();

        [Header("Icons")]
        [SerializeField] private List<PerkIconEntry> perkIcons = new();

        [Header("Dependencies")]
        [SerializeField] private PlayerPerks playerPerks;

        private readonly List<PerkType> orderedPerks = new();
        private Dictionary<PerkType, Sprite> iconLookup;

        private void Awake()
        {
            iconLookup = new Dictionary<PerkType, Sprite>();
            foreach (var entry in perkIcons)
            {
                if (entry.Icon != null)
                {
                    iconLookup[entry.Perk] = entry.Icon;
                }
            }

            SetSlotsVisible(false);
        }

        private void OnEnable()
        {
            if (playerPerks == null)
            {
                playerPerks = FindFirstObjectByType<PlayerPerks>();
            }

            if (playerPerks == null)
            {
                return;
            }

            playerPerks.PerkAdded += HandlePerkAdded;
            playerPerks.PerkRemoved += HandlePerkRemoved;
            playerPerks.PerksCleared += HandlePerksCleared;

            InitializeFromOwnedPerks();
        }

        private void OnDisable()
        {
            if (playerPerks == null)
            {
                return;
            }

            playerPerks.PerkAdded -= HandlePerkAdded;
            playerPerks.PerkRemoved -= HandlePerkRemoved;
            playerPerks.PerksCleared -= HandlePerksCleared;
        }

        private void InitializeFromOwnedPerks()
        {
            orderedPerks.Clear();
            foreach (var perk in playerPerks.OwnedPerks)
            {
                orderedPerks.Add(perk);
            }

            RefreshSlots();
        }

        private void HandlePerkAdded(PerkType perk)
        {
            if (orderedPerks.Contains(perk))
            {
                return;
            }

            orderedPerks.Add(perk);
            RefreshSlots();
        }

        private void HandlePerkRemoved(PerkType perk)
        {
            if (orderedPerks.Remove(perk))
            {
                RefreshSlots();
            }
        }

        private void HandlePerksCleared()
        {
            orderedPerks.Clear();
            RefreshSlots();
        }

        private void RefreshSlots()
        {
            for (var i = 0; i < perkSlots.Count; i++)
            {
                if (i < orderedPerks.Count && iconLookup.TryGetValue(orderedPerks[i], out var icon))
                {
                    perkSlots[i].sprite = icon;
                    perkSlots[i].gameObject.SetActive(true);
                }
                else
                {
                    perkSlots[i].sprite = null;
                    perkSlots[i].gameObject.SetActive(false);
                }
            }
        }

        private void SetSlotsVisible(bool visible)
        {
            foreach (var slot in perkSlots)
            {
                if (slot != null)
                {
                    slot.gameObject.SetActive(visible);
                }
            }
        }
    }
}
