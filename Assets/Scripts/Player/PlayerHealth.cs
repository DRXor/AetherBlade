using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Shield Settings")]
    public float currentShield = 0f;
    public float maxShield = 50f;

    [Header("Invulnerability")]
    public float invulnerabilityDuration = 0.5f;
    private bool isInvulnerable = false;

    [Header("Events")]
    public UnityEvent OnDamage;
    public UnityEvent OnDeath;
    public UnityEvent OnHeal;
    public UnityEvent OnShieldChange;

    [Header("Buff Settings")]
    public float damageMultiplier = 1f;

    void Start()
    {
        currentHealth = maxHealth;
        currentShield = 0f;
        damageMultiplier = 1f;

        Debug.Log($"HEALTH RESET IN START: {currentHealth}/{maxHealth}");
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        float remainingDamage = damage * damageMultiplier;

        if (currentShield > 0)
        {
            float shieldDamage = Mathf.Min(currentShield, remainingDamage);
            currentShield -= shieldDamage;
            remainingDamage -= shieldDamage;
            OnShieldChange?.Invoke();
        }

        if (remainingDamage > 0)
        {
            currentHealth -= remainingDamage;
        }

        OnDamage?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityFrames());
            StartCoroutine(DamageFlash());
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        currentShield = 0f;
        damageMultiplier = 1f;
        isInvulnerable = false;

        OnHeal?.Invoke(); 
        OnShieldChange?.Invoke();
        Debug.Log($"Health reset to {currentHealth}/{maxHealth}");
    }

    IEnumerator InvulnerabilityFrames()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    IEnumerator DamageFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color original = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = original;
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        OnHeal?.Invoke();
        Debug.Log($"Вылечен! Здоровье: {currentHealth}");
    }

    public void AddShield(float amount)
    {
        currentShield += amount;
        if (currentShield > maxShield) currentShield = maxShield;

        OnShieldChange?.Invoke();
        Debug.Log($"Щит увеличен: {currentShield}/{maxShield}");
    }

    public void ApplyDamageBuff(float multiplier, float duration)
    {
        StartCoroutine(DamageBuffCoroutine(multiplier, duration));
    }

    IEnumerator DamageBuffCoroutine(float multiplier, float duration)
    {
        float original = damageMultiplier;
        damageMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        damageMultiplier = original;
    }

    public void ApplyInvincibility(float duration)
    {
        StartCoroutine(InvincibilityCoroutine(duration));
    }

    IEnumerator InvincibilityCoroutine(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }

    void Die()
    {
        Debug.Log("ИГРОК УМЕР!");
        OnDeath?.Invoke();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayGameOverSound();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            SceneManager.LoadScene(0);
        }

        Destroy(gameObject);
    }

    public void UpdateUI()
    {
        OnHeal?.Invoke();
        OnShieldChange?.Invoke();
    }
}