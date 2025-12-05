using UnityEngine;

public class SimpleEnemyPatrol : MonoBehaviour
{
    [Header("Точки патрулирования")]
    public Transform pointA;
    public Transform pointB;

    [Header("Настройки")]
    public float speed = 1.5f;
    public float stoppingDistance = 0.3f;

    [Header("Преследование")]
    public float chaseRange = 4f;
    public float chaseSpeed = 2.5f;
    public float attackRange = 2.0f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    // Ссылки
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    // Состояние
    private bool movingToB = true;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();  // Добавляем получение Animator

        if (pointA == null || pointB == null)
            Debug.LogError("Назначь точки патрулирования!");
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }
            else
            {
                Attack();
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        Transform target = movingToB ? pointB : pointA;
        MoveToTarget(target.position, speed);

        if (Vector2.Distance(transform.position, target.position) < stoppingDistance)
        {
            movingToB = !movingToB;
        }

        if (spriteRenderer != null)
        {
            Vector2 direction = target.position - transform.position;
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    void ChasePlayer()
    {
        MoveToTarget(player.position, chaseSpeed);

        if (spriteRenderer != null)
        {
            Vector2 direction = player.position - transform.position;
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    void Attack()
    {
        // Останавливаемся для атаки
        transform.position = transform.position;

        // Поворот спрайта к игроку
        if (spriteRenderer != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            spriteRenderer.flipX = direction.x < 0;
        }

        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            // Запускаем анимацию атаки
            if (anim != null)
            {
                anim.SetBool("IsAttacking", true);
                isAttacking = true;
                Debug.Log($"{name}: Запускаю анимацию атаки");

                // Отключаем анимацию атаки через время
                Invoke("ResetAttackAnimation", 1.2f); // Увеличь если анимация длиннее
            }

            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"{name}: Атаковал игрока на {attackDamage} урона!");
            }

            lastAttackTime = Time.time;
        }
    }

    void ResetAttackAnimation()
    {
        if (anim != null)
        {
            anim.SetBool("IsAttacking", false);
            isAttacking = false;
            Debug.Log($"{name}: Сбрасываю анимацию атаки");
        }
    }

    void MoveToTarget(Vector2 target, float moveSpeed)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        // Рисуем точки и маршрут
        if (pointA != null && pointB != null)
        {
            // Точки
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pointA.position, 0.3f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(pointB.position, 0.3f);

            // Линия между точками
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);

            // Линия от врага к цели
            Gizmos.color = Color.yellow;
            Transform target = movingToB ? pointB : pointA;
            Gizmos.DrawLine(transform.position, target.position);
        }

        // Рисуем зону атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Рисуем зону преследования
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}