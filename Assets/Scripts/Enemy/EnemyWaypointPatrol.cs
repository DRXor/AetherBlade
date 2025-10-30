using UnityEngine;

public class EnemyWaypointPatrol : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waitTime = 1f;

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            return;
        }

        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        Vector2 targetPosition = waypoints[currentWaypointIndex].position;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Поворот спрайта
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }

        // Проверка достижения точки
        float distance = Vector2.Distance(transform.position, targetPosition);
        if (distance < 0.1f)
        {
            isWaiting = true;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (waypoints == null) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }
    }
}