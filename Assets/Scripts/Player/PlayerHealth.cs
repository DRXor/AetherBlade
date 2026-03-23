using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Invulnerability")]
    public float invulnerabilityDuration = 1.5f;
    public bool isInvulnerable = false;

    [Header("Events")]
    public UnityEvent OnDamage;
    public UnityEvent OnDeath;
    public UnityEvent OnHeal;

    [Header("Visual Feedback")]
    public Color damageColor = Color.red;
    public Color shieldDamageColor = Color.cyan;
    public float flashDuration = 0.1f;

    [Header("Buff Settings")]
    public float damageMultiplier = 1f;
    public bool isBuffActive = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Shield shieldComponent;
    private Coroutine invincibilityBuffCoroutine;
    private bool damageInvulnerability = false;
    private Coroutine buffVisualCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable || damageInvulnerability) return;

        currentHealth -= damage;

        // ВРЕМЕННО УБИРАЕМ АУДИОМЕНЕДЖЕР - ЕГО НЕТ В ПРОЕКТЕ
        // AudioManager.instance.PlaySound(0, 0.8f);

        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageEffect());
        }
    }

    IEnumerator DamageEffect()
    {
        damageInvulnerability = true;

        StartCoroutine(FlashEffect(Color.red));
        yield return new WaitForSeconds(invulnerabilityDuration);

        damageInvulnerability = false;
    }

    IEnumerator FlashEffect(Color flashColor)
    {
        float elapsedTime = 0f;
        float flashInterval = 0.1f;

        while (elapsedTime < invulnerabilityDuration && isInvulnerable)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = flashColor;
                yield return new WaitForSeconds(flashInterval);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(flashInterval);
            }
            elapsedTime += flashInterval * 2;
        }
    }

    void Die()
    {
        // ВРЕМЕННО УБИРАЕМ АУДИОМЕНЕДЖЕР
        // AudioManager.instance.PlaySound(1, 1f);

        Debug.Log($"{gameObject.name} died!");

        // Проверяем, игрок ли это (по тегу)
        if (gameObject.CompareTag("Player"))
        {
            // Вызываем GameOver через GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                Debug.LogError("GameManager.Instance is null! Add GameManager to scene.");
                // Запасной вариант: просто перезагрузить сцену
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            // Враг или другой объект — просто уничтожаем
            Destroy(gameObject);
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

        // ВРЕМЕННО УБИРАЕМ АУДИОМЕНЕДЖЕР
        // AudioManager.instance.PlaySound(2, 0.7f);

        Debug.Log($"{gameObject.name} healed. Health: {currentHealth}/{maxHealth}");
    }

    public void ApplyDamageBuff(float multiplier, float duration)
    {
        StartCoroutine(DamageBuffCoroutine(multiplier, duration));
    }

    private IEnumerator DamageBuffCoroutine(float multiplier, float duration)
    {
        damageMultiplier = multiplier;
        isBuffActive = true;

        // старт визуала
        if (buffVisualCoroutine != null)
            StopCoroutine(buffVisualCoroutine);

        buffVisualCoroutine = StartCoroutine(BuffVisual(new Color(0.7f, 0.3f, 1f))); // фиолетовый

        yield return new WaitForSeconds(duration);

        damageMultiplier = 1f;
        isBuffActive = false;

        // стоп визуала
        if (buffVisualCoroutine != null)
            StopCoroutine(buffVisualCoroutine);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    public void ApplyInvincibility(float duration)
    {
        if (invincibilityBuffCoroutine != null)
        {
            StopCoroutine(invincibilityBuffCoroutine);
        }

        invincibilityBuffCoroutine = StartCoroutine(InvincibilityBuffCoroutine(duration));
    }

    private IEnumerator InvincibilityBuffCoroutine(float duration)
    {
        isInvulnerable = true;

        // старт визуала
        if (buffVisualCoroutine != null)
            StopCoroutine(buffVisualCoroutine);

        buffVisualCoroutine = StartCoroutine(BuffVisual(Color.cyan));

        yield return new WaitForSeconds(duration);

        isInvulnerable = false;

        // стоп визуала
        if (buffVisualCoroutine != null)
            StopCoroutine(buffVisualCoroutine);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    IEnumerator BuffVisual(Color color)
    {
        while (true)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}