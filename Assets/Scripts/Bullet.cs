using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 10f;
    public float lifeTime = 3f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);

        // Автонастройка коллайдера пули
        SetupBulletCollider();
    }

    void Update()
    {
        // Движение пули
        if (rb != null)
        {
            rb.linearVelocity = transform.right * speed;
        }
        else
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }

    void SetupBulletCollider()
    {
        // Добавляем коллайдер если его нет
        Collider2D existingCollider = GetComponent<Collider2D>();
        if (existingCollider == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true; // Пули должны быть триггерами!
            collider.radius = 0.1f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ВАЖНО: Проверяем ВСЕ возможные теги врага
        if (other.CompareTag("Enemy") || other.CompareTag("enemy") || other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log($"Bullet hit: {other.gameObject.name}");

            HealthEnemy enemyHealth = other.GetComponent<HealthEnemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Damage dealt: {damage}");
            }
            else
            {
                // Пробуем найти HealthEnemy в родительских объектах
                enemyHealth = other.GetComponentInParent<HealthEnemy>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                    Debug.Log($"Damage dealt (parent): {damage}");
                }
            }

            Destroy(gameObject);
        }
        // Также уничтожаем пулю при столкновении со стенами
        else if (other.CompareTag("Wall") || other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    // ДУБЛИРУЮЩАЯ ПРОВЕРКА через Collision
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log($"Bullet collision with: {collision.gameObject.name}");
            Destroy(gameObject);
        }
    }
}