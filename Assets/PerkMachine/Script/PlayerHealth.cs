using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public bool hasQuickRevive = false;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Health: " + currentHealth + "/" + maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Took " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (hasQuickRevive)
        {
            Debug.Log("Quick Revive activated! Reviving...");
            currentHealth = maxHealth / 2;
            hasQuickRevive = false;
        }
        else
        {
            Debug.Log("PLAYER DIED");
        }
    }
}