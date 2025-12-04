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
    public float attackRange = 1.2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    [Header("Информация")]
    public bool movingToB = true;
    public float distanceToTarget;

    private SpriteRenderer spriteRenderer;
    private Transform player;
    private float lastAttackTime;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Проверяем точки
        if (pointA == null)
            Debug.LogError($"{name}: Точка A не назначена!");
        if (pointB == null)
            Debug.LogError($"{name}: Точка B не назначена!");

        if (pointA != null)
            Debug.Log($"{name}: Точка A на {pointA.position}");
        if (pointB != null)
            Debug.Log($"{name}: Точка B на {pointB.position}");

        // Начинаем с позиции между точками
        if (pointA != null)
            transform.position = pointA.position;
    }

    void Update()
    {
        // Если точек нет - выходим
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning($"{name}: Нет точек для патрулирования!");
            return;
        }

        // Если игрока нет - просто патрулируем
        if (player == null)
        {
            Patrol();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Атака
            Attack();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // Преследование
            Chase();
        }
        else
        {
            // Патрулирование
            Patrol();
        }
    }

    void Patrol()
    {
        // Выбираем цель
        Transform target = movingToB ? pointB : pointA;

        // Вычисляем направление
        Vector2 direction = (target.position - transform.position).normalized;

        // Двигаемся к цели
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Поворачиваем спрайт
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }

        // Проверяем расстояние до цели
        distanceToTarget = Vector2.Distance(transform.position, target.position);

        // Если достигли цели - меняем направление
        if (distanceToTarget < stoppingDistance)
        {
            Debug.Log($"{name}: Достиг точку {(movingToB ? "B" : "A")}! Меняю направление.");
            movingToB = !movingToB;
        }
    }

    void Chase()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            chaseSpeed * Time.deltaTime
        );

        // Поворачиваемся к игроку
        if (spriteRenderer != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    void Attack()
    {
        if (spriteRenderer != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            spriteRenderer.flipX = direction.x < 0;
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"{name}: Атаковал игрока на {attackDamage} урона!");
            }

            lastAttackTime = Time.time;
        }
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
    }
}