using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 25f;
    public float speed = 15f;
    public float lifeTime = 2f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ВАЖНО: Игнорируем столкновения с игроком
        if (collision.gameObject.CompareTag("Player"))
        {
            // Не уничтожаем пулю при столкновении с игроком
            // Пуля проходит сквозь игрока
            return;
        }

        Debug.Log($"Bullet collision with: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Bullet hit ENEMY!");

            HealthEnemy enemyHealth = collision.gameObject.GetComponent<HealthEnemy>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Damage {damage} dealt to {collision.gameObject.name}");
            }
        }

        // Уничтожаем пулю только при столкновении не с игроком
        Destroy(gameObject);
    }

    // Добавь этот метод для отладки
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Bullet passed through Player");
            // Ничего не делаем - пуля проходит сквозь игрока
        }
    }
}