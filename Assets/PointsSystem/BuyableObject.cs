using UnityEngine;
using Assets.UI.Scripts;

public class BuyableObject : MonoBehaviour
{
    [Header("Purchase Settings")]
    [SerializeField] private int cost = 0;
    [SerializeField] private string objectName = "Object";
    [SerializeField] private InteractionType interactionType = InteractionType.BuyWeapon;

    [Header("Object to Purchase")]
    [SerializeField] private GameObject objectToPurchase;
    [SerializeField] private bool disableAfterPurchase = true;

    private bool playerNear = false;
    private Player currentPlayer;

    private void Update()
    {
        if (playerNear && currentPlayer != null)
        {
            UpdateHUD();

            var input = currentPlayer.GetComponent<PlayerInputHandler>();
            if (input != null && input.InteractPressed)
            {
                AttemptPurchase();
            }
        }
    }

    private void UpdateHUD()
    {
        if (HUDManager.Instance != null)
        {
            string message = GetPromptText();
            HUDManager.Instance.SetInteractText(message, true);
        }
    }

    private string GetPromptText()
    {
        // Using same presets as InteractionPrompt.cs
        return interactionType switch
        {
            InteractionType.BuyWeapon => $"Hold E to buy {objectName} weapon for [Cost: {cost}]",
            InteractionType.UnlockDoor => $"Hold E to unlock door for [Cost: {cost}]",
            InteractionType.PickupItem => $"Hold E to pickup {objectName} [Cost: {cost}]",
            InteractionType.BuyPerk => $"Hold E to buy {objectName} for [Cost: {cost}]",
            _ => $"Hold E to purchase {objectName} [Cost: {cost}]"
        };
    }

    private void AttemptPurchase()
    {
        var points = currentPlayer.GetComponent<PlayerPoints>();
        if (points == null) return;

        if (interactionType == InteractionType.BuyWeapon)
        {
            var weapons = currentPlayer.GetComponent<PlayerWeapons>();
            if (weapons == null) return;

            if (weapons.HasWeapon(objectToPurchase))
            {
                if (points.SpendPoints(cost))
                {
                    weapons.RefillWeaponAmmo(objectToPurchase);
                }
            }
            else if (weapons.OwnedWeapons.Count < weapons.MaxWeaponsOwned)
            {
                if (points.SpendPoints(cost))
                {
                    weapons.TryAddWeapon(objectToPurchase);
                }
            }
            else
            {
                Debug.Log("Inventory full!");
                return;
            }
        }
        else if (points.SpendPoints(cost))
        {
            if (objectToPurchase != null)
            {
                objectToPurchase.SetActive(true);
            }

            if (disableAfterPurchase)
            {
                if (HUDManager.Instance != null)
                {
                    HUDManager.Instance.SetInteractText("", false);
                }
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            playerNear = true;
            currentPlayer = player;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null && player == currentPlayer)
        {
            playerNear = false;
            currentPlayer = null;
            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.SetInteractText("", false);
            }
        }
    }
}
