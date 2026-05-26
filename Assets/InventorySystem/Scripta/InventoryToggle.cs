using UnityEngine;
public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryCanvas;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryCanvas.SetActive(!inventoryCanvas.activeSelf);
            UnityEngine.Debug.Log("Inventory toggled: " + inventoryCanvas.activeSelf);

        }
    }
}
