using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
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

        if (shieldComponent != null && shieldComponent.TakeDamage(damage))
        {
            if (spriteRenderer != null)
            {
                StartCoroutine(FlashColor(shieldDamageColor));
            }
            return;
        }

        currentHealth -= damage;
        OnDamage?.Invoke();

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashColor(damageColor));
        }

        Debug.Log($"{gameObject.name} took {damage} health damage. Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        OnHeal?.Invoke();
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    System.Collections.IEnumerator FlashColor(Color flashColor)
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
}