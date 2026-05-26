using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryPickup();
        }
    }

    private void TryPickup()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(inventory);
            }
        }
    }
}
