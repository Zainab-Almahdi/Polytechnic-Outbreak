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

        [Header("Container")]
        [SerializeField] private GameObject perksContainer;

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

            if (perksContainer == null && perkSlots.Count > 0 && perkSlots[0] != null)
            {
                perksContainer = perkSlots[0].transform.parent.gameObject;
            }
        }

        private void OnEnable()
        {
            if (playerPerks == null)
            {
                playerPerks = FindFirstObjectByType<PlayerPerks>();
            }

            if (playerPerks == null)
            {
                Debug.LogWarning("[PerkHUD] PlayerPerks dependency not found.");
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
            Debug.Log($"[PerkHUD] Perk added: {perk}");
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
            bool hasPerks = orderedPerks.Count > 0;
            if (perksContainer != null)
            {
                perksContainer.SetActive(hasPerks);
            }

            for (var i = 0; i < perkSlots.Count; i++)
            {
                if (i < orderedPerks.Count && iconLookup.TryGetValue(orderedPerks[i], out var icon))
                {
                    perkSlots[i].sprite = icon;
                    perkSlots[i].gameObject.SetActive(true);
                    Debug.Log($"[PerkHUD] Slot {i} set to {orderedPerks[i]}");
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
