using System;
using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
#if UNITY_EDITOR
    public int currentPoints = 10000;
#else
    public int currentPoints = 500;
#endif
    // Start with 10000 for testing change later to 500
    public int totalPointsEarned = 0;

    public event Action<int> PointsChanged;

    private void Start()
    {
        PointsChanged?.Invoke(currentPoints);
    }

    public void AddPoints(int amount)
    {
        currentPoints += amount;
        totalPointsEarned += amount;
        Debug.Log("+" + amount + " points! Total: " + currentPoints);
        PointsChanged?.Invoke(currentPoints);
    }

    public bool SpendPoints(int amount)
    {
        if (currentPoints >= amount)
        {
            currentPoints -= amount;
            Debug.Log("Spent " + amount + " points. Remaining: " + currentPoints);
            PointsChanged?.Invoke(currentPoints);
            return true;
        }
        else
        {
            Debug.Log("Not enough points! Need " + amount + ", have " + currentPoints);
            return false;
        }
    }

    void Update()
    {
        // TEST: Press P to add 1000 points (for testing)
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddPoints(1000);
        }
        
        // TEST: Press O to check points
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Current points: " + currentPoints);
        }
    }
}