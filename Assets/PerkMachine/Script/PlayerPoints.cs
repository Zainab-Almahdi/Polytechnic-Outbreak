using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
    public int currentPoints = 5000;  // Start with 5000 for testing
    public int totalPointsEarned = 0;

    public void AddPoints(int amount)
    {
        currentPoints += amount;
        totalPointsEarned += amount;
        Debug.Log("+" + amount + " points! Total: " + currentPoints);
    }

    public bool SpendPoints(int amount)
    {
        if (currentPoints >= amount)
        {
            currentPoints -= amount;
            Debug.Log("Spent " + amount + " points. Remaining: " + currentPoints);
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