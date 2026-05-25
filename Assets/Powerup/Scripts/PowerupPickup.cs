using System.Collections;
using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    // Set this in Inspector for each powerup object
    public string powerupName;

    // Optional pickup sound
    public AudioSource pickupSound;

    // How long to wait for sound before destroying
    public float destroyDelay = 0.5f;

    private bool pickedUp = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !pickedUp)
        {
            pickedUp = true;

            // TODO: Connect to PlayerPowerups once merged with main
            // Find the PlayerPowerups component on the player
            PlayerPowerups playerPowerups = other.GetComponent<PlayerPowerups>();
            if (playerPowerups != null)
            {
                playerPowerups.PickupPowerup(powerupName);
            }
            else
            {
                Debug.Log("PickupPowerup called with: " + powerupName);
            }

            // Play sound if assigned
            if (pickupSound != null)
            {
                pickupSound.Play();
            }

            // Destroy after delay
            StartCoroutine(DestroyAfterDelay());
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        // Hide the object immediately
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Wait for sound to finish
        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}