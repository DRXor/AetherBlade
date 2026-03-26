using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    private Animator anim;
    private Rigidbody2D rb;
    private int coin;
    private Vector2 moveInput;

    private float originalSpeed;

    public AudioClip[] footstepSounds;
    private float stepTimer;
    public float stepDelay = 0.4f;

    // Для отслеживания последней нажатой клавиши
    private string lastPressedKey = "";

    private bool isRecoiling = false;
    private float recoilTimer = 0f;
    private Vector2 recoilVelocity = Vector2.zero;

    [Header("Sprite Setup")]
    public bool autoSetupCollider = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.gravityScale = 0f;
        }

        anim = GetComponent<Animator>();
        coin = 0;

        if (CoinCount != null)
        {
            CoinCount.text = $"Coin: {coin}";
        }

        SetupSpriteAndCollider();
        originalSpeed = moveSpeed;
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
        }
    }

    void Update()
    {
        // Получаем ввод для движения
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(horizontal, vertical);
        if (moveInput.magnitude > 1) moveInput.Normalize();

        // ========== ОТСЛЕЖИВАЕМ ПОСЛЕДНЮЮ НАЖАТУЮ КЛАВИШУ ==========
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            lastPressedKey = "up";
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            lastPressedKey = "down";
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            lastPressedKey = "left";
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            lastPressedKey = "right";
        }

        // Проверяем, не отпустили ли последнюю нажатую клавишу
        if (lastPressedKey == "up" && !(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)))
        {
            // Ищем другую зажатую клавишу
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) lastPressedKey = "down";
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) lastPressedKey = "left";
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) lastPressedKey = "right";
            else lastPressedKey = "";
        }
        else if (lastPressedKey == "down" && !(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) lastPressedKey = "up";
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) lastPressedKey = "left";
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) lastPressedKey = "right";
            else lastPressedKey = "";
        }
        else if (lastPressedKey == "left" && !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) lastPressedKey = "up";
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) lastPressedKey = "down";
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) lastPressedKey = "right";
            else lastPressedKey = "";
        }
        else if (lastPressedKey == "right" && !(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) lastPressedKey = "up";
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) lastPressedKey = "down";
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) lastPressedKey = "left";
            else lastPressedKey = "";
        }

        // Определяем, двигается ли персонаж
        bool isMoving = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);

        if (isMoving)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                if (footstepSounds.Length > 0)
                {
                    int i = Random.Range(0, footstepSounds.Length);
                    AudioManager.instance.PlaySound(footstepSounds[i]);
                }

                stepTimer = Random.Range(0.5f, 0.7f); //задержка между шагами
            }
        }
        else
        {
            stepTimer = 0f;
        }

        // Отправляем в аниматор
        if (anim != null)
        {
            if (isMoving && lastPressedKey != "")
            {
                // Отправляем направление в зависимости от последней нажатой клавиши
                switch (lastPressedKey)
                {
                    case "up":
                        // W = вверх экрана, должен идти спиной (анимация Up)
                        anim.SetFloat("MoveX", 0);
                        anim.SetFloat("MoveY", 1);
                        break;
                    case "down":
                        // S = вниз экрана, должен идти лицом (анимация Down)
                        anim.SetFloat("MoveX", 0);
                        anim.SetFloat("MoveY", -1);
                        break;
                    case "left":
                        anim.SetFloat("MoveX", -1);
                        anim.SetFloat("MoveY", 0);
                        break;
                    case "right":
                        anim.SetFloat("MoveX", 1);
                        anim.SetFloat("MoveY", 0);
                        break;
                }
                anim.SetFloat("Speed", 1f);
            }
            else
            {
                anim.SetFloat("MoveX", 0);
                anim.SetFloat("MoveY", 0);
                anim.SetFloat("Speed", 0f);
            }
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        if (isRecoiling)
        {
            recoilTimer -= Time.fixedDeltaTime;

            rb.linearVelocity = recoilVelocity;
            recoilVelocity *= 0.88f;

            if (recoilTimer <= 0f)
            {
                isRecoiling = false;
                recoilVelocity = Vector2.zero;
            }
            return;
        }

        // Обычное движение
        Vector2 targetVelocity = moveInput * moveSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 13f * Time.fixedDeltaTime);

        if (moveInput.magnitude < 0.1f)
        {
            rb.linearVelocity *= 0.84f;

            if (rb.linearVelocity.magnitude < 0.06f)
                rb.linearVelocity = Vector2.zero;
        }
    }

    public void TriggerRecoil(Vector2 direction, float force, float duration = 0.26f)
    {
        if (rb == null) return;

        isRecoiling = true;
        recoilTimer = duration;
        recoilVelocity = direction * force;

        rb.AddForce(direction * force * 28f, ForceMode2D.Impulse);

        Debug.Log($"RECOIL ACTIVATED | X:{direction.x:F2} Y:{direction.y:F2} | Force:{force}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.pickupSound);

            coin += 1;
            if (CoinCount != null)
            {
                CoinCount.text = $"Coin: {coin}";
            }
            Destroy(collision.gameObject);
        }
    }

    public void AddCoin(int amount = 1)
    {
        coin += amount;
        if (CoinCount != null)
        {
            CoinCount.text = $"Coin: {coin}";
        }
    }

    public int GetCoinCount()
    {
        return coin;
    }

    public void ApplySpeedBuff(float multiplier, float duration)
    {
        StartCoroutine(SpeedBuffCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedBuffCoroutine(float multiplier, float duration)
    {
        moveSpeed = originalSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalSpeed;
    }
}