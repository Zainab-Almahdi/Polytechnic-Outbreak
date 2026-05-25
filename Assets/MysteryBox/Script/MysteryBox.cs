using System.Collections;
using UnityEngine;

public class MysteryBox : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private GameObject[] weapons; // Array to store possible weapons

    [Header("Lid")]
    [SerializeField] private Transform lid; // Reference to the lid transform
    [SerializeField] private float lidOpenAngle = -80f; // Angle to open the lid
    [SerializeField] private float lidOpenSpeed = 2f; // Speed of lid opening

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints; // Possible locations for the box to move

    [Header("Weapon Spawn")]
    [SerializeField] private Transform weaponSpawnPoint; // Where the weapon appears

    [Header("Chance")]
    [SerializeField] private int emptyChance = 20; // Chance to get nothing (0-100)

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Audio source for sounds
    [SerializeField] private AudioClip openSound; // Sound when box opens
    [SerializeField] private AudioClip emptySound; // Sound when box is empty

    [Header("Light")]
    [SerializeField] private GameObject boxLight; // Light effect for the box

    [Header("Effiects")]
    [SerializeField] private ParticleSystem disapperEffect; // Particle effect when box disappears

    [Header("Animation")]
    [SerializeField] private Animator animator; // Animator for box animation

    private bool used = false; // To check if box is already used
    private Quaternion closedRotation; // Store closed rotation of lid
    private Quaternion openRotation; // Store open rotation of lid
    private bool playerNear = false; // To check if player is near the box

    private void Start()
    {
        // Save the closed and open rotations for the lid
        closedRotation = lid.localRotation;
        openRotation = Quaternion.Euler(lid.localEulerAngles + new Vector3(lidOpenAngle, 0, 0));
    }

    private void Update()
    {
        // Smoothly rotate the lid open or closed
        Quaternion targetRotation = used ? openRotation : closedRotation;
        lid.localRotation = Quaternion.Slerp(lid.localRotation, targetRotation, Time.deltaTime * lidOpenSpeed);

        // If player is near and box is not used, check for input to use the box
        if (playerNear && !used && Input.GetKeyDown(KeyCode.E))
        {
            UseBox();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // When player enters the trigger, set playerNear to true
        if (other.CompareTag("Player"))
        {
            playerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When player leaves the trigger, set playerNear to false
        if (other.CompareTag("Player"))
        {
            playerNear = false;

        }
    }

    public void UseBox()
    {
        // If box is already used, do nothing
        if (used) return;

        used = true; // Mark box as used

        // Play open sound first
        if (audioSource != null && openSound != null)
        {
            audioSource.clip = openSound;
            audioSource.Play();
        }

        // Activate the Light
        if (boxLight != null)
        {
            boxLight.SetActive(true);
        }
      
        int random = Random.Range(0, 100); // Get a random number for chance
        // Empty Result
        if (random < emptyChance)
        {
            // Set light color to red if possible
            var lightComponent = boxLight != null ? boxLight.GetComponent<Light>() : null;
            if (lightComponent != null)
            {
                lightComponent.color = Color.red;
            }
            // Play empty sound after open sound
            if (audioSource != null && emptySound != null)
            {
                audioSource.clip = emptySound;
                audioSource.PlayDelayed(openSound.length);
            }
        }
        else
        {
            GiveRandomWeapon(); // Give player a random weapon
        }
        StartCoroutine(CloseAndMoveBox()); // Start coroutine to close and move the box
    }

    private IEnumerator CloseAndMoveBox()
    {
        // Wait for 5 seconds before closing
        yield return new WaitForSeconds(5f);
        used = false; // trigger the lid to close 
        // Wait for lid to close visually
        yield return new WaitForSeconds(1f);
        disapperEffect.gameObject.SetActive(true); // Show disappear effect
        animator.Play("Shrinking"); // Play shrinking animation
        yield return new WaitForSeconds(1f);
        MoveBox(); // Move the box to a new location
        disapperEffect.gameObject.SetActive(false); // Hide disappear effect
    }

    private void GiveRandomWeapon()
    {
        // Pick a random weapon from the array
        int randomIndex = UnityEngine.Random.Range(0, weapons.Length);
        GameObject weaponPrefab = weapons[randomIndex];
        // Instantiate the weapon at the spawn point
        Instantiate(weaponPrefab, weaponSpawnPoint.position, Quaternion.identity);
    }

    private void MoveBox()
    {
        // Move the box to a new random spawn point
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform newSpawnPoint = spawnPoints[randomIndex];
        transform.position = newSpawnPoint.position;
        used = false; // Reset box for next use
    }
}
