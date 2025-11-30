using UnityEngine;
using System.Collections;

public class UniversalEnemy : MonoBehaviour
{
    [Header("=== PATROL SETTINGS ===")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 1.5f;
    public float waitTimeAtPoints = 1f;

    [Header("=== COMBAT SETTINGS ===")]
    public float chaseSpeed = 2.5f;
    public float detectionRange = 5f;
    public float attackRange = 1.2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    public float stoppingDistance = 0.05f;

    [Header("=== PHYSICS SETTINGS ===")]
    public LayerMask obstacleLayers = 1; // ДОБАВЛЕНО! Теперь ошибки не будет
    public float collisionCheckDistance = 0.2f;
    public float playerAvoidanceDistance = 0f;

    [Header("=== COMPONENTS ===")]
    public HealthEnemy healthSystem;

    // Private variables
    private Transform player;
    private Rigidbody2D rb;
    private Health playerHealth;
    private int currentPatrolIndex = 0;
    private float lastAttackTime;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isWaiting = false;
    private Vector2 lastKnownPlayerPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthEnemy>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
        }

        // Автонастройка коллайдера если нужно
        SetupCollider();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Определяем состояние (патруль или преследование)
        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange + stoppingDistance)
        {
            isChasing = true;
            ChasePlayer();
        }
        else if (distanceToPlayer <= attackRange + stoppingDistance)
        {
            isChasing = true;
            AttackBehavior();
        }
        else
        {
            isChasing = false;
            PatrolBehavior();
        }
    }

    void PatrolBehavior()
    {
        if (patrolPoints == null || patrolPoints.Length == 0 || isWaiting) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        if (targetPoint == null) return;

        Vector2 direction = (targetPoint.position - transform.position).normalized;

        if (CanMoveToPosition(direction))
        {
            rb.linearVelocity = direction * patrolSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Достигли точки патруля
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        if (CanMoveToPosition(direction) && Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            rb.linearVelocity = direction * chaseSpeed;
            lastKnownPlayerPosition = player.position;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Поворачиваемся к игроку
        FacePlayer();
    }

    void AttackBehavior()
    {
        // Останавливаемся для атаки
        rb.linearVelocity = Vector2.zero;
        FacePlayer();

        // Атакуем если прошел кд
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackSequence());
            lastAttackTime = Time.time;
        }
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;

        Debug.Log("Enemy preparing to attack...");

        // Задержка перед атакой (windup)
        yield return new WaitForSeconds(0.3f);

        // Наносим урон игроку
        if (playerHealth != null && Vector2.Distance(transform.position, player.position) <= attackRange + 0.5f)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy attacked for " + attackDamage + " damage!");
        }

        isAttacking = false;
    }

    IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(waitTimeAtPoints);

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        isWaiting = false;
    }

    bool CanMoveToPosition(Vector2 direction)
    {
        if (direction == Vector2.zero) return true;

        // Используем несколько лучей для лучшего покрытия
        Vector2[] rayOrigins = new Vector2[] {
        Vector2.zero,
        new Vector2(0, 0.05f),
        new Vector2(0, -0.05f),
        new Vector2(0.05f, 0),
        new Vector2(-0.05f, 0)
    };

        foreach (Vector2 offset in rayOrigins)
        {
            Vector2 rayOrigin = (Vector2)transform.position + offset;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, collisionCheckDistance, obstacleLayers);

            // Визуализация лучей в редакторе
            Debug.DrawRay(rayOrigin, direction * collisionCheckDistance, Color.cyan, 0.1f);

            if (hit.collider != null && hit.collider != GetComponent<Collider2D>())
            {
                // Игнорируем триггеры и игрока
                if (hit.collider.isTrigger || hit.collider.CompareTag("Player"))
                    continue;

                Debug.Log($"Movement blocked by: {hit.collider.gameObject.name}");
                return false;
            }
        }

        return true;
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void SetupCollider()
    {
        // Автоматическая настройка коллайдера если его нет
        Collider2D existingCollider = GetComponent<Collider2D>();
        if (existingCollider == null)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = false; // ВАЖНО: не триггер!

            // Настройка размера под спрайт
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                collider.size = spriteRenderer.bounds.size;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Обработка столкновений со стенами
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Debug.Log($"Enemy collided with wall: {collision.gameObject.name}");

            // Небольшой отскок от стен
            Vector2 bounceDirection = -collision.contacts[0].normal;
            rb.AddForce(bounceDirection * 2f, ForceMode2D.Impulse);
        }

        // Столкновение с игроком - дополнительный урон
        if (collision.gameObject.CompareTag("Player"))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null && Time.time >= lastAttackTime + attackCooldown)
            {
                health.TakeDamage(attackDamage * 0.5f); // Меньший урон при столкновении
                lastAttackTime = Time.time;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Визуализация в редакторе
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange + stoppingDistance);

        // Отображение пути патрулирования
        if (patrolPoints != null && patrolPoints.Length > 1)
        {
            Gizmos.color = Color.green;
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