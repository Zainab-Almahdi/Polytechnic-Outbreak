using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab;

    private bool playerInRange = false;
    private WeaponSwitcher ws;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E PRESSED - PICKUP");

            if (ws != null)
            {
                ws.AddWeapon(weaponPrefab);
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER ENTERED RANGE");

            playerInRange = true;
            ws = other.GetComponent<WeaponSwitcher>();

            if (ws == null)
                Debug.LogError("WeaponSwitcher NOT FOUND ON PLAYER");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER LEFT RANGE");

            playerInRange = false;
            ws = null;
        }
    }
}