using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public Slider healthSlider;
    public Health playerHealth;

    [Header("Visual Options")]
    public bool changeColor = true;
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;

    private Image fillImage;

    void Start()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<Health>();

        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.currentHealth;

            if (changeColor)
                fillImage = healthSlider.fillRect.GetComponent<Image>();
        }

        if (playerHealth != null)
        {
            playerHealth.OnDamage.AddListener(UpdateHealthBar);
            playerHealth.OnHeal.AddListener(UpdateHealthBar);
        }
    }

    void Update()
    {
        if (playerHealth != null && healthSlider != null)
        {
            healthSlider.value = playerHealth.currentHealth;
            UpdateColor();
        }
    }

    void UpdateHealthBar()
    {
        if (healthSlider != null && playerHealth != null)
        {
            healthSlider.value = playerHealth.currentHealth;
            UpdateColor();
        }
    }

    void UpdateColor()
    {
        if (fillImage != null && changeColor)
        {
            float healthPercent = playerHealth.currentHealth / playerHealth.maxHealth;
            fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        }
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDamage.RemoveListener(UpdateHealthBar);
            playerHealth.OnHeal.RemoveListener(UpdateHealthBar);
        }
    }
}