using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            IsDead();
        }
    }

    void IsDead()
    {
        animator.SetTrigger("IsDead");
        Destroy(gameObject, 3f);
    }
}