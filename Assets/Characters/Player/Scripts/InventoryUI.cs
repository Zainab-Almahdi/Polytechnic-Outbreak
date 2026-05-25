using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Transform slotParent;
    public GameObject slotPrefab;

    private List<GameObject> slots = new List<GameObject>();

    public void UpdateUI(List<ItemData> items)
    {
        ClearUI();

        foreach (ItemData item in items)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);
            slots.Add(slot);

            slot.GetComponent<Image>().sprite = item.icon;
        }
    }

    void ClearUI()
    {
        foreach (GameObject s in slots)
        {
            Destroy(s);
        }

        slots.Clear();
    }
}