using UnityEngine;
using UnityEngine.AI;

public class CatBossAI : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public NavMeshAgent agent;
    public Animator animator;

    [Header("Boss Stats")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Ranges")]
    public float detectRange = 12f;
    public float attackRange = 2.5f;

    [Header("Attack")]
    public float attackCooldown = 2f;

    private float lastAttackTime;
    private bool isDead;
    private bool isHurting;

    private Transform currentTarget;

    void Start()
    {
        currentHealth = maxHealth;

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead)
            return;

        // 🔥 ADD THIS CHECK FIRST
        if (agent == null || !agent.isOnNavMesh)
        {
            return;
        }

        currentTarget = GetNearestPlayer();

        if (currentTarget == null)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (isHurting)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            return;
        }

        if (distance > detectRange)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
        }
        else if (distance > attackRange)
        {
            agent.isStopped = false;

            // 🔥 SAFE CALL
            if (agent.isOnNavMesh)
                agent.SetDestination(currentTarget.position);

            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        else
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);

            Vector3 lookPos = currentTarget.position - transform.position;
            lookPos.y = 0f;

            if (lookPos != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(lookPos),
                    8f * Time.deltaTime
                );
            }

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                animator.SetTrigger("Hit");
            }
        }
    }

    Transform GetNearestPlayer()
    {
        Transform nearest = null;
        float nearestDistance = Mathf.Infinity;

        if (player1 != null)
        {
            float d1 = Vector3.Distance(transform.position, player1.position);
            if (d1 < nearestDistance)
            {
                nearestDistance = d1;
                nearest = player1;
            }
        }

        if (player2 != null)
        {
            float d2 = Vector3.Distance(transform.position, player2.position);
            if (d2 < nearestDistance)
            {
                nearestDistance = d2;
                nearest = player2;
            }
        }

        return nearest;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            isDead = true;
            if (agent.isOnNavMesh)
                agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Dead"); // deadcat
            return;
        }

        isHurting = true;
        agent.isStopped = true;
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Hurt"); // hurt
        Invoke(nameof(EndHurt), 0.7f);
    }

    void EndHurt()
    {
        if (!isDead)
            isHurting = false;
    }
}