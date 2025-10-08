using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float acceleration = 18f;
    public float friction = 8f;
    public float drag = 6f; // Аэродинамическое сопротивление
    public float maxSpeed = 9f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Получаем ввод от клавиатуры
        moveInput.x = Input.GetAxisRaw("Horizontal"); 
        moveInput.y = Input.GetAxisRaw("Vertical");   
        
        // Нормализуем, чтобы диагональное движение не было быстрее
        moveInput = moveInput.normalized;
    }
    

    void FixedUpdate()
    {
        // Вычисляем желаемую скорость
        Vector2 targetVelocity = moveInput * moveSpeed;

        // Вычисляем разнице между желаемой и текущей скоростью
        Vector2 velocityDiff = targetVelocity - rb.linearVelocity;

        // Вычисляем силу для движения
        Vector2 movement = velocityDiff * acceleration;

        // Применяем силу к физическому телу
        rb.AddForce(movement);

        // ОГРАНИЧЕНИЕ МАКСИМАЛЬНОЙ СКОРОСТИ
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Трение, когда не двигаемся
        if (moveInput.magnitude < 0.1f)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, friction * Time.fixedDeltaTime);
        }


        // Естественное трение через физический drag
        if (moveInput.magnitude < 0.1f)
        {
            rb.linearVelocity *= (1f - drag * Time.fixedDeltaTime);

            // Полная остановка при очень малой скорости
            if (rb.linearVelocity.magnitude < 0.1f)
                rb.linearVelocity = Vector2.zero;
        }
    }
}
