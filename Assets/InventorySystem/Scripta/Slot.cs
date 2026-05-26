using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hovered;
    private ItemSO holdItem;
    private int itemAmount;
    public Image iconImage;
    private TextMeshProUGUI amountTxt;

    private void Awake()
    {
        iconImage = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        amountTxt = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        
    }
    public ItemSO GetItem()
    {
        return holdItem;
    }
    public int GetAmount()
    {
        return itemAmount;
    }
    public void SetItem(ItemSO item, int amount = 1)
    {
        holdItem = item;
        itemAmount = amount;
        UpdateSlot();
    }
    public void UpdateSlot() 
    {
        
        
        if (holdItem != null)
        {
                iconImage.enabled = true;
                iconImage.sprite = holdItem.Icon;
            iconImage.gameObject.SetActive(true);
            amountTxt.gameObject.SetActive(true); 
            amountTxt.text = itemAmount.ToString();
                 
            }
            else
            {
                iconImage.enabled = false;
                amountTxt.text = "";
            }
        }
        public int AddAmount(int amount)
    {
        itemAmount += amount;
        UpdateSlot();
        return itemAmount;
    }
    public int RemoveAmount(int amount)
    {
        itemAmount -= amount;
        if (itemAmount <= 0)
        {
            ClearSlot();
        }
        else
        {
            UpdateSlot();
        }
        return itemAmount;
    
}
    public void ClearSlot()
    {
        holdItem = null;
        itemAmount = 0;
        UpdateSlot();
    }
    public bool HasItem()
    {
        return holdItem != null;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}