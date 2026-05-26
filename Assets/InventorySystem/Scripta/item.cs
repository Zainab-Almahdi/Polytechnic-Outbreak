using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public ItemSO item;
    public int amount = 1;

    public void Interact(InventoryManager inventory)
    {
        inventory.AddItem(item, amount);
        Destroy(gameObject);
        inventory.EquipHandItem();
    }
}
