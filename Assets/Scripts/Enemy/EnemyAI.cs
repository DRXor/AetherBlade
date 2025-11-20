using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float moveSpeed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    private Transform player;
    private Rigidbody2D rb;
    private float lastAttackTime;
    private Health playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player != null)
            playerHealth = player.GetComponent<Health>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Преследование игрока
        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else if (distanceToPlayer <= attackRange)
        {
            // Останавливаемся для атаки
            rb.linearVelocity = Vector2.zero;

            // Атакуем если прошел кд
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void AttackPlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy attacked player for " + attackDamage + " damage!");
        }
    }

    // Визуализация в редакторе
    void OnDrawGizmosSelected()
    {
        // Область обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Область атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
