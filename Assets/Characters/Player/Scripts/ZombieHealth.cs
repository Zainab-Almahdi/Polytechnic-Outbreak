using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Animator animator;

    private ZombieDropper zombieDropper;

    void Start()
    {
        currentHealth = maxHealth;
        zombieDropper = GetComponent<ZombieDropper>();
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

        if (zombieDropper != null)
        {
        zombieDropper.OnZombieDeath();
        }

        Destroy(gameObject, 3f);
    }
}