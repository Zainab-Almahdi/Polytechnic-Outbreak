using UnityEngine;
using System;

public class ZombieHealth : MonoBehaviour
{
    public int health = 100;
    public Action OnDeath;

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();
        gameObject.SetActive(false); // hides it (pool-friendly)
    }
}