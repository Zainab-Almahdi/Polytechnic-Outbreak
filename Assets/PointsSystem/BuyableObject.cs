using UnityEngine;
using TMPro;
using System.Security.Cryptography;

public class BuyableObject : MonoBehaviour
{
    [Header("Purchase Settings")]
    [SerializeField] private int cost = 0;
    [SerializeField] private string objectName = "Object";

    [Header("Object to Purchase")]
    [SerializeField] private GameObject objectToPurchase;
    [SerializeField] private Transform objectToPurchaseTransform;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI promptText;
    private bool playerInside = false;

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            AttemptPurchase();
        }

    }
    private void AttemptPurchase()
    {
        Points.Instance.RemovePoints(cost);
        objectToPurchase.SetActive(true);
        promptText.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Vector3 objectPosition = objectToPurchaseTransform.position + new Vector3(0, 2f, 0); // 2 units above the objectToPurchase
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(objectPosition);
            promptText.rectTransform.position = screenPosition;
            promptText.gameObject.SetActive(true);
            promptText.text = $"Press E to buy {objectName} XP{cost}";

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            promptText.gameObject.SetActive(false);

        }
    }
}
