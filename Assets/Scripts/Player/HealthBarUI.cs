using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider;
    public Health playerHealth;

    private Image fillImage;

    void Start()
    {
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<Health>();

        if (playerHealth != null)
        {
            playerHealth.OnDamage.AddListener(UpdateHealthBar);
            playerHealth.OnHeal.AddListener(UpdateHealthBar);
        }

        if (healthSlider != null && playerHealth != null)
        {
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.currentHealth;
            fillImage = healthSlider.fillRect?.GetComponent<Image>();
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
        if (fillImage != null)
        {
            float percent = playerHealth.currentHealth / playerHealth.maxHealth;
            fillImage.color = Color.Lerp(Color.red, Color.green, percent);
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