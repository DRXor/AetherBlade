using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;

    void Start()
    {
        // Находим Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        // ЗАЩИТА ОТ ОШИБОК
        if (rb == null)
        {
            Debug.LogError("Bullet: Rigidbody2D not found! Please add Rigidbody2D component.");
            return;
        }

        // Движение пули
        rb.linearVelocity = transform.right * speed;

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
        Destroy(gameObject);
    }
}
