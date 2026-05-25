using UnityEngine;
using TMPro;
public class Points : MonoBehaviour
{
  public static Points Instance;
    [Header("Player Points ")]
    [SerializeField] private  int currentPoints = 1000;

    [Header("Points UI")]
    [SerializeField] private TextMeshProUGUI pointsText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        pointsText.text = "XP: " + currentPoints;


    }
    public void AddPoints(int amount)
    {
        currentPoints += amount;
        UpdateUI();
       UnityEngine.Debug.Log("Added " + amount + " points. Current points: " + currentPoints + ")");
    }
    public bool HaveEnoughPoints(int amount)
    {
        return currentPoints >= amount;
    }
    public void RemovePoints(int amount)
    {
        if (HaveEnoughPoints(amount))
        {
            currentPoints -= amount;
            UnityEngine.Debug.Log("Removed " + amount + " points. Current points: " + currentPoints);
            UpdateUI();
        }
        else
        {
            UnityEngine.Debug.LogWarning("Not enough points to remove!");
        }
    }
    public int GetCurrentPoints()
    {
        return currentPoints;
    }
    private void UpdateUI()
    {
        if (pointsText != null)
        {
            pointsText.text = "XP: " + currentPoints;
        }
    }
}
