using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ZombieAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private PlayerHealth playerHealth;
    private ZombieHealth zombieHealth;
    private float lastAttackTime;
    private bool isDead = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        zombieHealth = GetComponent<ZombieHealth>();
    }

    private void Start()
    {
        // Finding player - assuming there is only one PlayerHealth in the scene
        playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        
        if (playerHealth == null)
        {
            Debug.LogWarning($"Zombie {gameObject.name} could not find PlayerHealth in the scene.");
        }
    }

    private void Update()
    {
        if (isDead || playerHealth == null) return;

        // Check if we are currently playing the Attack animation
        bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") || animator.IsInTransition(0);

        if (isAttacking)
        {
            // During the actual animation, we stay rooted
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerHealth.transform.position);

        // Always update destination while not attacking
        agent.SetDestination(playerHealth.transform.position);

        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            // Only stop and trigger if we are ready to swing again
            agent.isStopped = true;
            TryAttack();
        }
        else
        {
            // Resume movement if we are either too far or just waiting for cooldown
            // This allows the zombie to "follow" the player closely even between attacks
            agent.isStopped = false;
        }

        // Update Animator Speed
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
    }

    private void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            playerHealth.Damage(attackDamage);
            lastAttackTime = Time.time;
        }
    }

    public void OnDeath()
    {
        isDead = true;
        
        ZombieAudio zombieAudio = GetComponent<ZombieAudio>();
        if (zombieAudio != null)
        {
            zombieAudio.OnDeath();
        }

        // Disable root motion so the animation doesn't move the transform
if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            
            // Reset velocity to prevent any physics pops or root motion leftovers
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
