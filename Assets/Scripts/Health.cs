using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isInvulnerable = false;

    [Header("Events")]
    public UnityEvent OnDamage;      // ??????? ??? ????????? ?????
    public UnityEvent OnDeath;       // ??????? ??? ??????
    public UnityEvent OnHeal;        // ??????? ??? ???????

    [Header("Visual Feedback")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;
        OnDamage?.Invoke();

        // ?????????? ?????? ????????? ?????
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}");

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

        // ????????? ??????? - ????? ???????? ?? ???????? ??????
        Destroy(gameObject);
    }

    System.Collections.IEnumerator FlashRed()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
}