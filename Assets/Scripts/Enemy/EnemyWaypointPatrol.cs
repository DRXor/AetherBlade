using UnityEngine;

public class EnemyWaypointPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 2.5f;
    public float chaseRange = 4f;
    public float attackRange = 1.2f;

    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    [Header("Advanced Settings")]
    public float acceleration = 2f;
    public float stoppingDistance = 0.05f; // Уменьшено!

    [Header("Debug")]
    public bool drawGizmos = true;

    private int currentPointIndex = 0;
    private Transform player;
    private bool isChasing = false;
    private float currentSpeed = 0f;
    private Rigidbody2D rb;
    private Health playerHealth;
    private float lastAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();

        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            if (playerHealth == null)
            {
                Debug.LogWarning("Health component not found on Player!");
            }
        }
        else
        {
            Debug.LogError("Player not found! Make sure player has 'Player' tag.");
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            transform.position = patrolPoints[0].position;
        }
    }

    void Update()
    {
        if (player == null || playerHealth == null)
        {
            TryFindPlayer();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Плавное изменение скорости
        float targetSpeed = 0f;

        if (distanceToPlayer <= attackRange + stoppingDistance)
        {
            // Останавливаемся для атаки
            targetSpeed = 0f;
            AttackPlayer();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // Преследование с плавным ускорением
            targetSpeed = chaseSpeed;
            isChasing = true;
        }
        else
        {
            // Патрулирование
            targetSpeed = patrolSpeed;
            isChasing = false;
        }

        // Плавное изменение скорости
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // Движение
        if (isChasing && distanceToPlayer > attackRange + stoppingDistance)
        {
            ChasePlayer();
        }
        else if (!isChasing)
        {
            Patrol();
        }
    }

    void TryFindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        if (targetPoint == null) return;

        Vector2 newPosition = Vector2.MoveTowards(transform.position, targetPoint.position, currentSpeed * Time.deltaTime);

        if (rb != null)
            rb.MovePosition(newPosition);
        else
            transform.position = newPosition;

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        Vector2 newPosition = Vector2.MoveTowards(transform.position, player.position, currentSpeed * Time.deltaTime);

        if (rb != null)
            rb.MovePosition(newPosition);
        else
            transform.position = newPosition;
    }

    void AttackPlayer()
    {
        // Атакуем только если прошел кулдаун
        if (Time.time >= lastAttackTime + attackCooldown && playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Enemy attacked player for {attackDamage} damage!");
            lastAttackTime = Time.time;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Если враг сталкивается с игроком - наносим урон сразу
        if (collision.gameObject.CompareTag("Player"))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(attackDamage * 0.5f);
                Debug.Log($"Enemy collided with player for {attackDamage * 0.5f} damage!");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (patrolPoints != null && patrolPoints.Length > 1)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);
                    int nextIndex = (i + 1) % patrolPoints.Length;
                    if (patrolPoints[nextIndex] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
                    }
                }
            }
        }
    }
}