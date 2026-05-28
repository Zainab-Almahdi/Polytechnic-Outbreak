using UnityEngine;

public class PerkMachine : MonoBehaviour
{
    public int perkCost = 2500;
    public PerkType perkType = PerkType.HealthIncrease;
    public string perkName = "Perk";
    public AudioSource buySound;

    private bool playerNearby = false;
    private GameObject player;
    private PlayerPoints playerPoints;
    private PlayerPerks playerPerks;
    private PlayerInputHandler playerInput;

    private void Update()
    {
        if (playerNearby && playerInput != null && playerInput.InteractPressed)
        {
            BuyPerk();
        }
    }

    private void BuyPerk()
    {
        if (playerPerks == null) return;

        if (playerPerks.HasPerk(perkType))
        {
            Debug.Log($"{perkName} already owned!");
            return;
        }

        if (playerPoints != null && playerPoints.SpendPoints(perkCost))
        {
            if (playerPerks.TryAddPerk(perkType))
            {
                Debug.Log($"Bought {perkName}!");
                if (buySound != null) buySound.Play();
            }
            else
            {
                // Refund if failed to add (e.g. limit reached)
                playerPoints.AddPoints(perkCost);
                Debug.Log($"Failed to add {perkName}. Points refunded.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            player = other.gameObject;
            playerPoints = player.GetComponent<PlayerPoints>();
            playerPerks = player.GetComponent<PlayerPerks>();
            playerInput = player.GetComponent<PlayerInputHandler>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            player = null;
            playerPoints = null;
            playerPerks = null;
            playerInput = null;
        }
    }
}

