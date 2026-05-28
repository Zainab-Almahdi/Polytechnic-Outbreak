using System.Collections;
using UnityEngine;
using Assets.UI.Scripts; // Ensure HUDManager is accessible

public class MysteryBox : MonoBehaviour
{
    private enum BoxState { ReadyToRoll, Rolling, ReadyToPickup, Moving }
    [SerializeField] private BoxState currentState = BoxState.ReadyToRoll;

    [Header("Settings")]
    [SerializeField] private int rollCost = 950;
    [SerializeField] private GameObject[] weapons; 
    [SerializeField] private float rollDuration = 3f;

    [Header("Lid")]
    [SerializeField] private Transform lid; 
    [SerializeField] private float lidOpenAngle = -80f; 
    [SerializeField] private float lidOpenSpeed = 2f; 

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints; 

    [Header("Weapon Spawn")]
    [SerializeField] private Transform weaponSpawnPoint; 
    [SerializeField] private Vector3 weaponPreviewRotationOffset;

    [Header("Chance")]
    [SerializeField] private int emptyChance = 10; 

    [Header("Pickup Timeout")]
    [SerializeField] private float pickupTimeoutSeconds = 10f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip openSound; 
    [SerializeField] private AudioClip emptySound; 

    [Header("Light")]
    [SerializeField] private GameObject boxLight; 

    [Header("Effiects")]
    [SerializeField] private ParticleSystem disapperEffect; 

    [Header("Animation")]
    [SerializeField] private Animator animator; 

    private Quaternion closedRotation; 
    private Quaternion openRotation; 
    private bool playerNear = false; 
    private Player currentPlayer;
    private GameObject currentSpawnedWeapon;
    private GameObject selectedWeaponPrefab;
    private Coroutine pickupTimeoutRoutine;

    private void Start()
    {
        closedRotation = lid.localRotation;
        openRotation = Quaternion.Euler(lid.localEulerAngles + new Vector3(lidOpenAngle, 0, 0));
        
        if (boxLight != null) boxLight.SetActive(false);
    }

    private void Update()
    {
        // Smoothly rotate the lid open or closed based on state
        bool lidShouldBeOpen = (currentState == BoxState.Rolling || currentState == BoxState.ReadyToPickup);
        Quaternion targetRotation = lidShouldBeOpen ? openRotation : closedRotation;
        lid.localRotation = Quaternion.Slerp(lid.localRotation, targetRotation, Time.deltaTime * lidOpenSpeed);

        if (playerNear && currentPlayer != null)
        {
            UpdateHUD();
            
            var input = currentPlayer.GetComponent<PlayerInputHandler>();
            if (input != null && input.InteractPressed)
            {
                HandleInteraction();
            }
        }
    }

    private void UpdateHUD()
    {
        if (HUDManager.Instance == null) return;

        string message = "";
        bool visible = true;

        switch (currentState)
        {
            case BoxState.ReadyToRoll:
                message = $"Press E to use Mystery Box [Cost: {rollCost}]";
                break;
            case BoxState.ReadyToPickup:
                string weaponName = "Weapon";
                if (selectedWeaponPrefab != null)
                {
                    var gun = selectedWeaponPrefab.GetComponent<Gun>();
                    weaponName = gun != null && !string.IsNullOrWhiteSpace(gun.displayName)
                        ? gun.displayName
                        : selectedWeaponPrefab.name;
                }
                // Clean up name if it has (Clone) or gun_ prefix
                weaponName = weaponName.Replace("gun_", "").Replace("(Clone)", "").Trim();
                message = $"Hold E to get {weaponName}";
                break;
            default:
                visible = false;
                break;
        }

        HUDManager.Instance.SetInteractText(message, visible);
    }

    private void HandleInteraction()
    {
        switch (currentState)
        {
            case BoxState.ReadyToRoll:
                var points = currentPlayer.GetComponent<PlayerPoints>();
                if (points != null && points.SpendPoints(rollCost))
                {
                    StartCoroutine(RollRoutine());
                }
                break;

            case BoxState.ReadyToPickup:
                var weaponsComp = currentPlayer.GetComponent<PlayerWeapons>();
                if (weaponsComp != null && weaponsComp.TryAddWeapon(selectedWeaponPrefab))
                {
                    CleanupPickup();
                }
                break;
        }
    }

    private IEnumerator RollRoutine()
    {
        currentState = BoxState.Rolling;
        if (HUDManager.Instance != null) HUDManager.Instance.SetInteractText("", false);

        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        if (boxLight != null) boxLight.SetActive(true);

        // Visual roll: cycle through weapons
        float elapsed = 0;
        float interval = 0.1f;
        int lastIdx = -1;

        while (elapsed < rollDuration)
        {
            int idx = Random.Range(0, weapons.Length);
            if (idx != lastIdx)
            {
                if (currentSpawnedWeapon != null) Destroy(currentSpawnedWeapon);
                currentSpawnedWeapon = Instantiate(weapons[idx], weaponSpawnPoint.position, weaponSpawnPoint.rotation);
                currentSpawnedWeapon.transform.rotation = weaponSpawnPoint.rotation * Quaternion.Euler(weaponPreviewRotationOffset);
                currentSpawnedWeapon.transform.localScale = Vector3.one * 0.2f;
                
                // Disable Gun and WeaponSway on preview
                var gun = currentSpawnedWeapon.GetComponent<Gun>();
                if (gun != null) gun.enabled = false;
                
                lastIdx = idx;
            }
            elapsed += interval;
            yield return new WaitForSeconds(interval);
            // Speed up the cycle slightly towards the end
            interval = Mathf.Lerp(0.1f, 0.3f, elapsed / rollDuration);
        }

        // Final result
        if (Random.Range(0, 100) < emptyChance)
        {
            HandleEmpty();
        }
        else
        {
            currentState = BoxState.ReadyToPickup;
            selectedWeaponPrefab = weapons[lastIdx];
            StartPickupTimeout();
        }
    }

    private void HandleEmpty()
    {
        if (currentSpawnedWeapon != null) Destroy(currentSpawnedWeapon);
        if (audioSource != null && emptySound != null) audioSource.PlayOneShot(emptySound);
        
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        currentState = BoxState.Moving;
        yield return new WaitForSeconds(2f); // Wait for teddy bear laugh
        
        if (disapperEffect != null)
        {
            disapperEffect.gameObject.SetActive(true);
            disapperEffect.Play();
        }
        
        if (animator != null) animator.Play("Shrinking");
        
        yield return new WaitForSeconds(1.5f);
        
        MoveBox();
        currentState = BoxState.ReadyToRoll;
        if (boxLight != null) boxLight.SetActive(false);
    }

    private void MoveBox()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform newSpawnPoint = spawnPoints[randomIndex];
            transform.position = newSpawnPoint.position;
            transform.rotation = newSpawnPoint.rotation;
        }
    }

    private void CleanupPickup()
    {
        if (currentSpawnedWeapon != null) Destroy(currentSpawnedWeapon);
        currentState = BoxState.ReadyToRoll;
        if (boxLight != null) boxLight.SetActive(false);
        StopPickupTimeout();
        
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.SetInteractText("", false);
        }
    }

    private void StartPickupTimeout()
    {
        StopPickupTimeout();
        pickupTimeoutRoutine = StartCoroutine(PickupTimeoutRoutine());
    }

    private void StopPickupTimeout()
    {
        if (pickupTimeoutRoutine != null)
        {
            StopCoroutine(pickupTimeoutRoutine);
            pickupTimeoutRoutine = null;
        }
    }

    private IEnumerator PickupTimeoutRoutine()
    {
        yield return new WaitForSeconds(pickupTimeoutSeconds);

        if (currentState != BoxState.ReadyToPickup)
        {
            pickupTimeoutRoutine = null;
            yield break;
        }

        if (currentSpawnedWeapon != null)
        {
            Destroy(currentSpawnedWeapon);
        }

        currentState = BoxState.ReadyToRoll;
        if (boxLight != null) boxLight.SetActive(false);
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.SetInteractText("", false);
        }

        pickupTimeoutRoutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Use GetComponentInParent to find Player component on root or character controller
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

    // Public method for backward compatibility if needed, though internal logic is now state-driven
    public void UseBox() { }
}
