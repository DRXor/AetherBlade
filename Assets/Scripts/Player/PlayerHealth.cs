using UnityEngine;
using UnityEngine.Events;
using System.Collections;

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

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Shield shieldComponent;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        shieldComponent = GetComponent<Shield>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        int intDamage = Mathf.RoundToInt(damage);
        currentHealth -= intDamage;

        // ЗВУК: Получение урона
        AudioManager.instance.PlaySound(0, 0.8f); // индекс 0 - звук получения урона

        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

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
        isInvulnerable = true;
        StartCoroutine(FlashEffect(Color.red));
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
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
        // ЗВУК: Смерть игрока
        AudioManager.instance.PlaySound(1, 1f); // индекс 1 - звук смерти
        Debug.Log("Player died! Game Over");
        Destroy(gameObject);
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

        // ЗВУК: Лечение
        AudioManager.instance.PlaySound(2, 0.7f); // индекс 2 - звук лечения

        Debug.Log("Player healed. Health: " + currentHealth + "/" + maxHealth);
    }

    System.Collections.IEnumerator FlashColor(Color flashColor)
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
}