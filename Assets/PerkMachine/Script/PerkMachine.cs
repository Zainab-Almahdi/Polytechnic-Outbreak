//using UnityEngine;

//public class PerkMachine : MonoBehaviour
//{
//    public int perkCost = 2500;
//    public string perkName = "Juggernog";
//    public GameObject promptText;
//    public AudioSource buySound;
//    private bool playerNearby = false;
//    private GameObject player;
//    private PlayerPoints playerPoints;
//    private bool hasBeenPurchased = false;

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

//    void BuyPerk()
//    {
//        if (hasBeenPurchased)
//        {
//            Debug.Log("Already bought " + perkName + "!");
//            return;
//        }

//        if (playerPoints == null || !playerPoints.SpendPoints(perkCost))
//        {
//            Debug.Log("Need " + perkCost + " points to buy " + perkName);
//            return;
//        }

//        hasBeenPurchased = true;

//        Debug.Log("Bought " + perkName + " perk!");

//        if (buySound != null)
//        {
//            buySound.Play();
//        }

//        // Apply perk effects
//        if (perkName == "Juggernog")
//        {
//            player.GetComponent<PlayerHealth>().maxHealth = 200;
//            player.GetComponent<PlayerHealth>().currentHealth = 200;
//            Debug.Log("Juggernog: Health increased to 200");
//        }
//        else if (perkName == "QuickRevive")
//        {
//            player.GetComponent<PlayerHealth>().hasQuickRevive = true;
//            Debug.Log("Quick Revive: Self-revive unlocked");
//        }
//        else if (perkName == "SpeedCola")
//        {
//            player.GetComponent<PlayerShooting>().reloadSpeed = 0.5f;
//            Debug.Log("Speed Cola: Reload speed doubled");
//        }
//        else if (perkName == "DoubleTap")
//        {
//            player.GetComponent<PlayerShooting>().fireRate = 0.1f;
//            Debug.Log("Double Tap: Fire rate increased");
//        }
//        else if (perkName == "StaminUp")
//        {
//            player.GetComponent<PlayerMovement>().speed = 10f;
//            Debug.Log("Stamin-Up: Movement speed increased");
//        }
//        else if (perkName == "MuleKick")
//        {
//            player.GetComponent<WeaponSwitcher>().maxWeapons = 3;
//            Debug.Log("Mule Kick: Third weapon slot unlocked");
//        }

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

//    void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player") && !hasBeenPurchased)
//        {
//            playerNearby = true;
//            player = other.gameObject;
//            if (promptText != null) promptText.SetActive(true);
//            Debug.Log("Press E to buy " + perkName + " (" + perkCost + " points)");
//            playerPoints = player.GetComponent<PlayerPoints>();
//        }
//    }

//    void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            playerNearby = false;
//            if (promptText != null) promptText.SetActive(false);
//        }
//    }
//}