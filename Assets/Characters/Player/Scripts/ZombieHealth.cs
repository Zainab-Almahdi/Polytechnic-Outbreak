using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Animator animator;

    private ZombieDropper zombieDropper;

    public bool isBoss = false;
    public static bool BossDead { get; private set; } = false;

    void Start()
    {
        currentHealth = maxHealth;
        zombieDropper = GetComponent<ZombieDropper>();
        
        if (isBoss)
        {
            BossDead = false;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            IsDead();
        }
    }

    private bool isDead = false;

    void IsDead()
    {
        if (isDead) return;
        isDead = true;

        if (isBoss)
        {
            BossDead = true;
        }

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
            GameManager.Instance.OnZombieKilled();

        var ai = GetComponent<ZombieAI>();
        if (ai != null)
            ai.OnDeath();

        Destroy(gameObject, 3f);
    }

    public void KillInstantly()
    {
        currentHealth = 0f;
        IsDead();
    }
}