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

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Shield shieldComponent;

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

        // ÂĞÅÌÅÍÍÎ ÓÁÈĞÀÅÌ ÀÓÄÈÎÌÅÍÅÄÆÅĞ - ÅÃÎ ÍÅÒ Â ÏĞÎÅÊÒÅ
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
        // ÂĞÅÌÅÍÍÎ ÓÁÈĞÀÅÌ ÀÓÄÈÎÌÅÍÅÄÆÅĞ
        // AudioManager.instance.PlaySound(1, 1f);

        Debug.Log($"{gameObject.name} died!");

        // Ïğîâåğÿåì, èãğîê ëè ıòî (ïî òåãó)
        if (gameObject.CompareTag("Player"))
        {
            // Âûçûâàåì GameOver ÷åğåç GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                Debug.LogError("GameManager.Instance is null! Add GameManager to scene.");
                // Çàïàñíîé âàğèàíò: ïğîñòî ïåğåçàãğóçèòü ñöåíó
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            // Âğàã èëè äğóãîé îáúåêò — ïğîñòî óíè÷òîæàåì
            Destroy(gameObject);
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

        // ÂĞÅÌÅÍÍÎ ÓÁÈĞÀÅÌ ÀÓÄÈÎÌÅÍÅÄÆÅĞ
        // AudioManager.instance.PlaySound(2, 0.7f);

        Debug.Log($"{gameObject.name} healed. Health: {currentHealth}/{maxHealth}");
    }
}