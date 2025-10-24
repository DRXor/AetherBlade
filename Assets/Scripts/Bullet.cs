using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 15f;
    public float damage = 25f;
    public float lifetime = 2f;

    private Rigidbody2D rb;
    private bool hasHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Bullet: Rigidbody2D not found!");
            return;
        }

        // Настройка Rigidbody для пули
        rb.gravityScale = 0f; // Без гравитации
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Непрерывное обнаружение коллизий
        rb.interpolation = RigidbodyInterpolation2D.Extrapolate; // Сглаживание движения

        // Движение пули
        rb.linearVelocity = transform.right * speed;

        // Уничтожение пули через время
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

        // Игнорируем столкновения с другими пулями
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            if (bullet != gameObject)
            {
                Collider2D otherBulletCollider = bullet.GetComponent<Collider2D>();
                Collider2D thisCollider = GetComponent<Collider2D>();
                if (otherBulletCollider != null && thisCollider != null)
                {
                    Physics2D.IgnoreCollision(thisCollider, otherBulletCollider);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return; // Предотвращаем множественные срабатывания
        hasHit = true;

        Debug.Log($"Bullet hit: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");

        // Наносим урон врагам
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ApplyDamageToEnemy(collision.gameObject);
        }

        // Визуальный эффект попадания (можно добавить частицы)
        CreateHitEffect();

        // Отключаем коллайдер и видимость перед уничтожением
        DisableBullet();

        // Уничтожаем с небольшой задержкой для эффекта
        Destroy(gameObject, 0.05f);
    }

    void ApplyDamageToEnemy(GameObject enemy)
    {
        HealthEnemy enemyHealth = enemy.GetComponent<HealthEnemy>();
        if (enemyHealth != null)
        {
            enemyHealth.take_damage_to_enemy(damage);
            Debug.Log($"SUCCESS: Enemy took {damage} damage!");
        }
        else
        {
            Debug.LogError("HealthEnemy component NOT found!");

            // Альтернативная попытка найти любой компонент здоровья
            Health universalHealth = enemy.GetComponent<Health>();
            if (universalHealth != null)
            {
                universalHealth.TakeDamage(damage);
                Debug.Log($"SUCCESS: Used universal Health component!");
            }
        }
    }

    void CreateHitEffect()
    {
        // Временный эффект - изменение цвета
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = Color.red;
        }
    }

    void DisableBullet()
    {
        // Отключаем компоненты чтобы пуля не мешала
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;

        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
}