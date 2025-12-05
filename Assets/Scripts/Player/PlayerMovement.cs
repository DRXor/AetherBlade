using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float acceleration = 18f;
    public float friction = 8f;
    public float drag = 6f;
    public float maxSpeed = 9f;

    public GameObject coinPrefab;
    public Text CoinCount;

    // ДОБАВЛЕНИЕ: Переменная для управления анимациями
    private Animator anim;

    private Rigidbody2D rb;
    private int coin;
    private Vector2 moveInput;

    [Header("Sprite Setup")]
    public bool autoSetupCollider = true;

    void Start()
    {
        // ИСПРАВЛЕНИЕ: Добавлена проверка и автоматическое создание Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody2D not found! Adding one automatically.");
            rb = gameObject.AddComponent<Rigidbody2D>();
            // Настройки по умолчанию для 2D платформера
            rb.freezeRotation = true;
            rb.gravityScale = 0f; // Для top-down игры
        }

        // ДОБАВЛЕНИЕ: Получаем компонент Animator для управления анимациями
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("Animator not found on player! Animation system may not work.");
        }

        coin = 0;

        // ИСПРАВЛЕНИЕ: Проверка на null для UI элемента
        if (CoinCount != null)
        {
            CoinCount.text = $"Coin: {coin}";
        }
        else
        {
            Debug.LogWarning("CoinCount Text not assigned in inspector!");
        }

        SetupSpriteAndCollider();
    }

    void SetupSpriteAndCollider()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        if (spriteRenderer != null && collider != null)
        {
            collider.size = spriteRenderer.bounds.size;
            collider.offset = new Vector2(0, 0);
        }
        else
        {
            Debug.LogWarning("SpriteRenderer or BoxCollider2D not found for auto-setup");
        }

        if (autoSetupCollider)
        {
            SetupCollider();
        }
    }

    void SetupCollider()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        if (sr != null && collider != null && sr.sprite != null)
        {
            collider.size = sr.sprite.bounds.size;
            collider.offset = Vector2.zero;
            Debug.Log($"Auto-setup collider for {gameObject.name}: {collider.size}");
        }
    }

    void Update()
    {
        // Получаем ввод от клавиатуры
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput = moveInput.normalized;

        // ДОБАВЛЕНИЕ: Передаем параметры направления движения в аниматор
        if (anim != null)
        {
            anim.SetFloat("MoveX", moveInput.x);
            anim.SetFloat("MoveY", moveInput.y);
        }
    }


    void FixedUpdate()
    {
        // ИСПРАВЛЕНИЕ: Проверка rb на null перед использованием
        if (rb == null) return;

        // Вычисляем желаемую скорость
        Vector2 targetVelocity = moveInput * moveSpeed;

        // Вычисляем разницу между желаемой и текущей скоростью
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

        // ДОБАВЛЕНИЕ: Передаем параметр скорости в аниматор (нормализованная скорость)
        if (anim != null)
        {
            float speed = rb.linearVelocity.magnitude / maxSpeed;
            anim.SetFloat("Speed", speed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            coin += 1;

            // ИСПРАВЛЕНИЕ: Проверка на null для UI
            if (CoinCount != null)
            {
                CoinCount.text = $"Coin: {coin}";
            }

            Destroy(collision.gameObject);

            // ЗВУК: Подбор монетки
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySound(5, 0.7f); // индекс 5 - звук монетки
            }
        }
    }

    // Дополнительный метод для сбора монет из других скриптов
    public void AddCoin(int amount = 1)
    {
        coin += amount;

        if (CoinCount != null)
        {
            CoinCount.text = $"Coin: {coin}";
        }

        // ЗВУК: Подбор монетки
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(5, 0.7f);
        }
    }

    // Метод для получения текущего количества монет
    public int GetCoinCount()
    {
        return coin;
    }
}