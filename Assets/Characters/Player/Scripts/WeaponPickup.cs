using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public ItemData item;

    private PlayerInventory inventory;
    private bool inRange;

    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            inventory.AddItem(item);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inventory = other.GetComponent<PlayerInventory>();
            inRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inventory = null;
            inRange = false;
        }
    }
}