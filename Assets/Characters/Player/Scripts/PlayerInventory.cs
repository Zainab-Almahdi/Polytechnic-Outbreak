using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();

    public WeaponSwitcher weaponSwitcher;
    public InventoryUI inventoryUI;

    void Awake()
    {
        weaponSwitcher = GetComponent<WeaponSwitcher>();
    }

    public void AddItem(ItemData item)
    {
        items.Add(item);

        Debug.Log("Picked up: " + item.itemName);

        weaponSwitcher.AddWeapon(item.prefab);

        inventoryUI.UpdateUI(items);
    }

    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
        inventoryUI.UpdateUI(items);
    }
}