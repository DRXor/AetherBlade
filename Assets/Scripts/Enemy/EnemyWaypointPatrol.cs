using UnityEngine;
using System.Collections;

public class EnemyWaypointPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 2.5f;
    public float chaseRange = 4f;
    public float attackRange = 1.2f;

    [Header("Advanced Settings")]
    public float acceleration = 2f;
    public float stoppingDistance = 0.5f;

    [Header("Debug")]
    public bool drawGizmos = true;

    private int currentPointIndex = 0;
    private Transform player;
    private EnemyAI enemyAI;
    private bool isChasing = false;
    private float currentSpeed = 0f;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemyAI = GetComponent<EnemyAI>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null) Debug.LogError("Player not found!");
        if (enemyAI == null) Debug.LogError("EnemyAI component not found!");

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            transform.position = patrolPoints[0].position;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Плавное изменение скорости
        float targetSpeed = 0f;

        if (distanceToPlayer <= attackRange)
        {
            // Останавливаемся для атаки
            targetSpeed = 0f;
            Attack();
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

    void Attack()
    {
        if (enemyAI != null)
        {
            enemyAI.AttackPlayer();
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