using UnityEngine;
[CreateAssetMenu(fileName = " Item", menuName = "Inventory/Item")]

public class ItemSO : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite Icon;
    public GameObject itemPrefab;
    public GameObject handItemPrefab;
    public int maxStackSize;



}
