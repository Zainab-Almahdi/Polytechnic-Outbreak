using UnityEngine;

public enum InteractionType
{
    BuyWeapon,
    UnlockDoor,
    PickupItem,
    BuyPerk
}

public class InteractionPrompt : MonoBehaviour
{
    public InteractionType interactionType;
    public string itemName;
    public int cost;

    public string GetPromptText()
    {
        return interactionType switch
        {
            InteractionType.BuyWeapon => $"Hold E to buy {itemName} weapon for [Cost: {cost}]",
            InteractionType.UnlockDoor => $"Hold E to unlock door for [Cost: {cost}]",
            InteractionType.PickupItem => $"Hold E to pickup {itemName}",
            InteractionType.BuyPerk => $"Hold E to buy {itemName} for [Cost: {cost}]",
            _ => string.Empty
        };
    }
}
