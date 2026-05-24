//using UnityEngine;

public class PerkMachine : MonoBehaviour
{
    [SerializeField] public int perkCost = 2500;
    [SerializeField] private PerkType perkType = PerkType.HealthIncrease;
    [SerializeField] public GameObject promptText;
    [SerializeField] public AudioSource buySound;
    [SerializeField] private bool playerNearby = false;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerPoints playerPoints;
    [SerializeField] private bool hasBeenPurchased = false;

//    private float startDelay = 1f;

//    void Update()
//    {
//        if (playerNearby && Input.GetKeyDown(KeyCode.E))
//        {
//            if (player != null && Vector3.Distance(transform.position, player.transform.position) < 3f)
//            {
//                BuyPerk();
//            }
//            else
//            {
//                playerNearby = false;
//                if (promptText != null) promptText.SetActive(false);
//            }
//        }
//    }

    void BuyPerk()
    {
        if (hasBeenPurchased)
        {
            Debug.Log("Already bought " + perkType + "!");
            return;
        }

        if (playerPoints == null || !playerPoints.SpendPoints(perkCost))
        {
            Debug.Log("Need " + perkCost + " points to buy " + perkType);
            return;
        }

//        hasBeenPurchased = true;

        Debug.Log("Bought " + perkType + " perk!");

//        if (buySound != null)
//        {
//            buySound.Play();
//        }

        // Apply perk effects via the perk system for modular modifiers.
        var perkSystem = player.GetComponent<PlayerPerks>();
        if (perkSystem != null)
        {
            if (perkSystem.TryAddPerk(perkType))
            {
                Debug.Log($"{perkType}: Perk added.");
            }
            else
            {
                Debug.Log($"{perkType}: Perk already owned or perk limit reached.");
            }
        }

//        // Hide prompt
//        if (promptText != null) promptText.SetActive(false);
      
//        Collider[] colliders = GetComponents<Collider>();
//        foreach (Collider col in colliders)
//        {
//            if (col.isTrigger)
//            {
//                col.enabled = false;
//                break;
//            }
//        }
//    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPurchased)
        {
            playerNearby = true;
            player = other.gameObject;
            if (promptText != null) promptText.SetActive(true);
            Debug.Log("Press E to buy " + perkType + " (" + perkCost + " points)");
            playerPoints = player.GetComponent<PlayerPoints>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (promptText != null) promptText.SetActive(false);
        }
    }

}
