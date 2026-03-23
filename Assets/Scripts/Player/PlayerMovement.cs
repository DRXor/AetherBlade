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

    private Animator anim;
    private Rigidbody2D rb;
    private int coin;
    private Vector2 moveInput;

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
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        if (anim != null)
        {
            anim.SetFloat("MoveX", moveInput.x);
            anim.SetFloat("MoveY", moveInput.y);
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector2 targetVelocity = moveInput * moveSpeed;
        Vector2 velocityDiff = targetVelocity - rb.linearVelocity;
        Vector2 movement = velocityDiff * acceleration;
        rb.AddForce(movement);

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        if (moveInput.magnitude < 0.1f)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, friction * Time.fixedDeltaTime);
        }

        if (moveInput.magnitude < 0.1f)
        {
            rb.linearVelocity *= (1f - drag * Time.fixedDeltaTime);
            if (rb.linearVelocity.magnitude < 0.1f)
                rb.linearVelocity = Vector2.zero;
        }

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
}