using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private readonly HashSet<string> accessCards = new();
    private readonly List<string> items = new();

    // Puzzle items are separate to support single active puzzle object logic.
    public string PuzzleItem { get; private set; }
    public IReadOnlyCollection<string> AccessCards => accessCards;
    public IReadOnlyList<string> Items => items;

    public bool AddAccessCard(string cardId)
    {
        return !string.IsNullOrWhiteSpace(cardId) && accessCards.Add(cardId);
    }

    public bool HasAccessCard(string cardId)
    {
        return !string.IsNullOrWhiteSpace(cardId) && accessCards.Contains(cardId);
    }

    public void SetPuzzleItem(string itemId)
    {
        PuzzleItem = itemId;
    }

    public void AddItem(string itemId)
    {
        if (!string.IsNullOrWhiteSpace(itemId))
        {
            items.Add(itemId);
        }
    }

    public bool RemoveItem(string itemId)
    {
        return items.Remove(itemId);
    }
}
