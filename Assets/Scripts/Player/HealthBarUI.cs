using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider;
    public Slider shieldSlider;
    private Health playerHealth;

    void Start()
    {
        playerHealth = FindFirstObjectByType<Health>();

        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<Health>();
            return;
        }

        if (healthSlider != null)
        {
            healthSlider.value = playerHealth.currentHealth;
            healthSlider.maxValue = playerHealth.maxHealth;
        }

        if (shieldSlider != null)
        {
            shieldSlider.value = playerHealth.currentShield;
            shieldSlider.maxValue = playerHealth.maxShield;
        }
    }
}