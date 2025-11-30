using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 15f;
    public float damage = 25f;
    public float lifetime = 2f;
    public float knockbackForce = 5f;

    [Header("Hit Effects")]
    public GameObject hitEffectPrefab;
    public AudioClip hitSound;

    private Rigidbody2D rb;
    private bool hasHit = false;
    private Vector2 initialDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Bullet: Rigidbody2D not found!");
            return;
        }

        // Настройка физики
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Сохраняем начальное направление для отбрасывания
        initialDirection = transform.right;
        rb.linearVelocity = initialDirection * speed;

        Destroy(gameObject, lifetime);
        SetupCollisionIgnore();
    }

    void SetupCollisionIgnore()
    {
        // Игнорируем столкновения с игроком
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D bulletCollider = GetComponent<Collider2D>();

            if (playerCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(bulletCollider, playerCollider);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;
        hasHit = true;

        // Наносим урон врагам
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ApplyDamageToEnemy(collision.gameObject, collision.contacts[0].point);
        }

        CreateHitEffect(collision.contacts[0].point);
        DisableBullet();
        Destroy(gameObject, 0.05f);
    }

    void ApplyDamageToEnemy(GameObject enemy, Vector2 hitPoint)
    {
        HealthEnemy enemyHealth = enemy.GetComponent<HealthEnemy>();
        if (enemyHealth != null)
        {
            enemyHealth.take_damage_to_enemy(damage);

            // Применяем отбрасывание
            ApplyKnockback(enemy, hitPoint);

            Debug.Log($"SUCCESS: Enemy took {damage} damage!");
        }
    }

    void ApplyKnockback(GameObject enemy, Vector2 hitPoint)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            // Направление отбрасывания - от точки попадания
            Vector2 knockbackDir = ((Vector2)enemy.transform.position - hitPoint).normalized;
            enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }
    }

    void CreateHitEffect(Vector2 position)
    {
        // Визуальный эффект попадания
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, position, Quaternion.identity);
        }

        // Звуковой эффект
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, position);
        }

        // Анимация попадания на самом спрайте пули
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = Color.red;
        }
    }

    void DisableBullet()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;

        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
}