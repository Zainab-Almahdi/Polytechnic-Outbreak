using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UI;
using System.Reflection;
using System;
public class InventoryManager : MonoBehaviour
{
  //  public static InventoryManager Instance;

    public GameObject hotbarObject;
    public GameObject inventorySlotParent;
    public Image DragIcon;
    public Transform hand;


    private List<Slot> inventorySlot = new List<Slot>();
    private List<Slot> hotbarSlot = new List<Slot>();
    private List<Slot> allSlot = new List<Slot>();
    private Slot draggedSlot = null;
    private bool dragged = false;
    private int equiped = 0;
    private float equipedOpacity = 0.9f;
    private float normalOpacity = 0.5f;
    private GameObject currentHandItem;


    private void Awake()
    {
        inventorySlot.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
        hotbarSlot.AddRange(hotbarObject.GetComponentsInChildren<Slot>());
        allSlot.AddRange(inventorySlot);
        allSlot.AddRange(hotbarSlot);
    }
    void Update()
    {
     
        startDrag();
        updateDragIcon();
       Drag();
        HandleHotbarSelection();
        UpdateHotbarOpacity();
        HandleDropEquipedItem();
    }
    public void AddItem(ItemSO item, int amount)
    {
        int remainingAmount = amount;
        foreach (Slot slot in allSlot)
        {
            if (slot.HasItem() && slot.GetItem() == item)
            {
                int currentAmount = slot.GetAmount();
                int maxStackSize = item.maxStackSize;
                if (currentAmount < maxStackSize)
                {
                    int spaceLeft = maxStackSize - currentAmount;
                    int amountToAdd = Mathf.Min(spaceLeft, remainingAmount);
                    slot.SetItem(item, currentAmount + amountToAdd);
                    remainingAmount -= amountToAdd;
                    if (remainingAmount <= 0)
                        return;
                }
            }
        }
        foreach (Slot slot in allSlot)
        {
            if (!slot.HasItem())
            {
                int amountToAdd = Mathf.Min(item.maxStackSize, remainingAmount);
                slot.SetItem(item, amountToAdd);
                remainingAmount -= amountToAdd;
                if (remainingAmount <= 0)
                    return;
            }
        }
        if (remainingAmount > 0)
        {
            //  Debug.Log("Not enough space in inventory for " + item.itemName + ". Remaining amount: " + remainingAmount);
        }
    }
    private void startDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Slot hoverd = GetHoveredSlot();
            if (hoverd != null && hoverd.HasItem())
            {
                draggedSlot = hoverd;
                dragged = true;

                DragIcon.sprite = hoverd.GetItem().Icon;
                DragIcon.color = Color.white;
                DragIcon.enabled = true;
            }


        }
    }
    private Slot GetHoveredSlot()
    {
        foreach (Slot slot in allSlot)
        {
            if (slot.hovered)
            {
                return slot;
            }
        }
        return null;
    }
    private void Drag()
    {
        if (Input.GetMouseButtonUp(0) && dragged)
        { 
                Slot hoverd = GetHoveredSlot();
                if (hoverd != null)
                {
                   HandleDrop(draggedSlot, hoverd);
                    DragIcon.enabled = false;
                    draggedSlot = null;
                    dragged = false;
                }
            } 
    }
    private void HandleDrop(Slot from, Slot to)
    {
        if (from == to) return;

        if (to.HasItem() && from.GetItem() == to.GetItem())
        {
            int maxStackSize = to.GetItem().maxStackSize;
            int space = maxStackSize - to.GetAmount();

            if (space > 0)
            {
                int amountToMove = Mathf.Min(space, from.GetAmount());
                to.SetItem(to.GetItem(), to.GetAmount() + amountToMove);
                from.SetItem(from.GetItem(), from.GetAmount() - amountToMove);
                if (from.GetAmount() <= 0)
                {
                    from.ClearSlot();
                }

            }
            return;
        }


        if (to.HasItem())
        {
            ItemSO tempItem = to.GetItem();
            int tempAmount = to.GetAmount();
            to.SetItem(from.GetItem(), from.GetAmount());
            from.SetItem(tempItem, tempAmount);
        }
        else
        {
            to.SetItem(from.GetItem(), from.GetAmount());
            from.ClearSlot();
            return;
        }
    }
    private void updateDragIcon()
    {
        if (dragged)
        {
            Vector2 mousePos = Input.mousePosition;
            DragIcon.transform.position = mousePos;
        }
}
      private void UpdateHotbarOpacity()
    {
        for (int i = 0; i < hotbarSlot.Count; i++)
        {
            Image slotImage = hotbarSlot[i].GetComponent<Image>();
            Color normalColor = slotImage.color;

            if (i == equiped && ColorUtility.TryParseHtmlString("#999B9B", out Color Silver))
            {


                slotImage.color = new Color(Silver.r, Silver.g, Silver.b, equipedOpacity);
            }
            else 
            { 


                slotImage.color = normalColor;
        }
               
            
        }
       


    }
    // Hotbar slot selection

    private void HandleHotbarSelection() 
    { 
    for (int i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                equiped = i;
                EquipHandItem();            }
        }
    }
    

    // Drop item
    private void HandleDropEquipedItem()
    {
        if (!Input.GetKeyDown(KeyCode.Q)) return;
        Slot equipedSlot = hotbarSlot[equiped];
        if (!equipedSlot.HasItem()) return;

        ItemSO itemSO = equipedSlot.GetItem();
        GameObject prefab = itemSO.itemPrefab;

        if(prefab == null) return;
        GameObject droppedItem = Instantiate(prefab, Camera.main.transform.position + Camera.main.transform.forward * 2, Quaternion.identity);

        Item item = droppedItem.GetComponentInChildren<Item>();
        UnityEngine.Debug.Log(item != null ? "Item component found" : "Item component not found");
        item.item = itemSO;
        UnityEngine.Debug.Log("ItemSO assigned to dropped item: " + (item.item != null ? item.item.itemName : "null"));
        item.amount = equipedSlot.GetAmount();
        equipedSlot.ClearSlot();
        EquipHandItem();
    }
    public void EquipHandItem()
    {
        if (currentHandItem != null)
        {
            Destroy(currentHandItem);
        }
        Slot equipedSlot = hotbarSlot[equiped];
        if (!equipedSlot.HasItem()) return;

        ItemSO itemSO = equipedSlot.GetItem();
        currentHandItem = Instantiate(itemSO.handItemPrefab, hand);
        currentHandItem.transform.localPosition = Vector3.zero;
        currentHandItem.transform.localRotation = Quaternion.identity;

    }
}