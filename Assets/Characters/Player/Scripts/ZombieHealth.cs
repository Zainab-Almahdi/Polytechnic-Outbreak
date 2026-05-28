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

        PlayerPoints points = Object.FindFirstObjectByType<PlayerPoints>();
        if (points != null)
        {
            points.AddPoints(80);
        }

        if (zombieDropper != null)
{
            zombieDropper.OnZombieDeath();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnZombieKilled();
        }

        var ai = GetComponent<ZombieAI>();
        if (ai != null)
        {
            ai.OnDeath();
        }
        Destroy(gameObject, 3f);
    }
}