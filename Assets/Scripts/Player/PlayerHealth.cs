using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Shield Settings")]
    public float currentShield = 0f;
    public float maxShield = 50f;

    [Header("Damage Buff")]
    public float damageMultiplier = 1f;

    [Header("Invulnerability")]
    public float invulnerabilityDuration = 0.5f;
    private bool isInvulnerable = false;

    [Header("Events")]
    public UnityEvent OnDamage;
    public UnityEvent OnHeal;
    public UnityEvent OnDeath;

    [Header("UI - НЕ ТРОГАТЬ!")]
    public Slider healthBar;
    public Slider shieldBar;

    void Start()
    {
        currentHealth = maxHealth;
        currentShield = 0f;
        damageMultiplier = 1f;

        // ПРИНУДИТЕЛЬНЫЙ ПОИСК
        ForceFindUI();

        UpdateUI();
    }

    void ForceFindUI()
    {
        // Ищем через Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            // Ищем HealthSlider в любом месте
            Slider[] sliders = canvas.GetComponentsInChildren<Slider>(true);
            foreach (Slider s in sliders)
            {
                if (s.name == "HealthBar" || s.name == "HealthSlider")
                {
                    healthBar = s;
                    Debug.Log("HealthBar найден: " + s.name);
                }
                if (s.name == "ShieldBar" || s.name == "ShieldSlider")
                {
                    shieldBar = s;
                    Debug.Log("ShieldBar найден: " + s.name);
                }
            }
        }

        // Если не нашли - ищем по имени
        if (healthBar == null)
        {
            GameObject hpBarObj = GameObject.Find("HealthBar");
            if (hpBarObj != null) healthBar = hpBarObj.GetComponent<Slider>();
        }
        if (shieldBar == null)
        {
            GameObject shBarObj = GameObject.Find("ShieldBar");
            if (shBarObj != null) shieldBar = shBarObj.GetComponent<Slider>();
        }

        // ВАЖНО: если healthBar всё ещё null - СОЗДАЁМ
        if (healthBar == null)
        {
            CreateHealthBar();
        }
    }

    void CreateHealthBar()
    {
        // Создаём Canvas если нет
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Создаём HealthBar
        GameObject hpBarObj = new GameObject("HealthBar");
        hpBarObj.transform.SetParent(canvas.transform, false);

        RectTransform rect = hpBarObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, 0.9f);
        rect.anchorMax = new Vector2(0.9f, 0.95f);
        rect.sizeDelta = Vector2.zero;

        healthBar = hpBarObj.AddComponent<Slider>();

        // Создаём фон
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(hpBarObj.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = Color.gray;
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Создаём заполнение
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(hpBarObj.transform, false);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = Color.green;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        healthBar.targetGraphic = fillImg;
        healthBar.fillRect = fillRect;

        Debug.Log("HealthBar создан автоматически!");
    }

    void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
            healthBar.maxValue = maxHealth;
            Debug.Log($"UI ОБНОВЛЕН: {currentHealth}/{maxHealth}");
        }
        else
        {
            Debug.LogError("HealthBar = NULL! Не могу обновить UI!");
        }

        if (shieldBar != null)
        {
            shieldBar.value = currentShield;
            shieldBar.maxValue = maxShield;
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
            Debug.Log($"Щит поглотил: {shieldDamage}. Осталось щита: {currentShield}");
        }

        if (remainingDamage > 0)
        {
            currentHealth -= remainingDamage;
            Debug.Log($"Урон по здоровью: {remainingDamage}. Здоровье: {currentHealth}/{maxHealth}");
        }

        UpdateUI(); // ЭТО ОБНОВЛЯЕТ ШКАЛУ
        OnDamage?.Invoke();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityFrames());
        }
    }

    IEnumerator InvulnerabilityFrames()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateUI();
        OnHeal?.Invoke();
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
            GameManager.Instance.GameOver();
        else
            SceneManager.LoadScene(0);
    }
}