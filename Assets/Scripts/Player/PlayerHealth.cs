using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("Buff Settings")]
    public float damageMultiplier = 1f;

    // UI элементы - автоматически найдутся
    private Slider healthSlider;
    private Slider shieldSlider;
    private Image healthFillImage;

    void Start()
    {
        currentHealth = maxHealth;
        currentShield = 0f;
        damageMultiplier = 1f;

        FindUIElements();
        UpdateUI();

        Debug.Log($"HEALTH RESET IN START: {currentHealth}/{maxHealth}");
    }

    void FindUIElements()
    {
        // Ищем сл��йдер здоровья
        GameObject healthCanvas = GameObject.Find("PlayerHealthCanvas");
        if (healthCanvas != null)
        {
            Transform bar = healthCanvas.transform.Find("HealthBar");
            if (bar != null)
            {
                healthSlider = bar.GetComponent<Slider>();
                if (healthSlider != null && healthSlider.fillRect != null)
                    healthFillImage = healthSlider.fillRect.GetComponent<Image>();
            }
        }

        if (healthSlider == null)
        {
            healthSlider = GameObject.Find("HealthBar")?.GetComponent<Slider>();
        }

        // Ищем слайдер щита
        GameObject shieldCanvas = GameObject.Find("ShieldCanvas");
        if (shieldCanvas != null)
        {
            Transform bar = shieldCanvas.transform.Find("ShieldBar");
            if (bar != null)
                shieldSlider = bar.GetComponent<Slider>();
        }

        if (shieldSlider == null)
        {
            shieldSlider = GameObject.Find("ShieldBar")?.GetComponent<Slider>();
        }

        Debug.Log($"HealthSlider found: {healthSlider != null}");
        Debug.Log($"ShieldSlider found: {shieldSlider != null}");
    }

    public void UpdateUI()
    {
        // Обновляем здоровье
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            healthSlider.maxValue = maxHealth;

            // Меняем цвет
            if (healthFillImage != null)
            {
                float percent = currentHealth / maxHealth;
                healthFillImage.color = Color.Lerp(Color.red, Color.green, percent);
            }
        }

        // Обновляем щит
        if (shieldSlider != null)
        {
            shieldSlider.value = currentShield;
            shieldSlider.maxValue = maxShield;
        }

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            healthSlider.maxValue = maxHealth;
            Debug.Log($"FORCE UPDATE: healthSlider.value = {currentHealth}/{maxHealth}"); // ПРОВЕРКА
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        float remainingDamage = damage;

        if (currentShield > 0)
        {
            float shieldDamage = Mathf.Min(currentShield, remainingDamage);
            currentShield -= shieldDamage;
            remainingDamage -= shieldDamage;
        }

        if (remainingDamage > 0)
        {
            currentHealth -= remainingDamage;
        }

        UpdateUI();
        OnDamage?.Invoke();  // ЭТО ВАЖНО - вызывает обновление HealthBarUI

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
        UpdateUI();
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
        UpdateUI();
        OnHeal?.Invoke();
        Debug.Log($"Вылечен! Здоровье: {currentHealth}");
    }

    public void AddShield(float amount)
    {
        currentShield += amount;
        if (currentShield > maxShield) currentShield = maxShield;
        UpdateUI();
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

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}