using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;
    public float waitTime = 1f;
    public LayerMask groundLayer;

    [Header("Debug")]
    public bool showGizmos = true;

    private Vector2 startPosition;
    private Vector2 leftPoint;
    private Vector2 rightPoint;
    private Vector2 targetPoint;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Запоминаем стартовую позицию
        startPosition = transform.position;

        // Рассчитываем точки патрулирования
        leftPoint = startPosition + Vector2.left * patrolDistance;
        rightPoint = startPosition + Vector2.right * patrolDistance;

        // Начинаем движение вправо
        targetPoint = rightPoint;

        // Автоматически определяем слой земли если не задан
        if (groundLayer == 0)
            groundLayer = LayerMask.GetMask("Default");
    }

    void Update()
    {
        if (isWaiting)
        {
            // Ожидание в точке
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                SwitchDirection();
            }
            return;
        }

        PatrolMovement();
        CheckGroundAhead();
    }

    void PatrolMovement()
    {
        // Движение к целевой точке
        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Поворот спрайта в направлении движения
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }

        // Проверка достижения целевой точки
        float distanceToTarget = Vector2.Distance(transform.position, targetPoint);
        if (distanceToTarget < 0.1f)
        {
            isWaiting = true;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void SwitchDirection()
    {
        // Смена направления патрулирования
        if (targetPoint == leftPoint)
            targetPoint = rightPoint;
        else
            targetPoint = leftPoint;
    }

    void CheckGroundAhead()
    {
        // Проверка наличия земли впереди чтобы не упасть с платформы
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.down * 0.5f;
        float rayDirection = spriteRenderer.flipX ? -1f : 1f;
        Vector2 rayEnd = rayOrigin + Vector2.right * rayDirection * 0.8f;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * rayDirection, 0.8f, groundLayer);

        if (hit.collider == null)
        {
            // Если земли нет - разворачиваемся
            SwitchDirection();
            isWaiting = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // Визуализация точек патрулирования
        Gizmos.color = Color.red;

        Vector2 currentStartPos = Application.isPlaying ? startPosition : (Vector2)transform.position;
        Vector2 currentLeft = Application.isPlaying ? leftPoint : currentStartPos + Vector2.left * patrolDistance;
        Vector2 currentRight = Application.isPlaying ? rightPoint : currentStartPos + Vector2.right * patrolDistance;

        Gizmos.DrawWireSphere(currentLeft, 0.2f);
        Gizmos.DrawWireSphere(currentRight, 0.2f);
        Gizmos.DrawLine(currentLeft, currentRight);

        // Визуализация проверки земли
        Gizmos.color = Color.blue;
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.down * 0.5f;
        float rayDirection = spriteRenderer != null && spriteRenderer.flipX ? -1f : 1f;
        Gizmos.DrawRay(rayOrigin, Vector2.right * rayDirection * 0.8f);
    }
}