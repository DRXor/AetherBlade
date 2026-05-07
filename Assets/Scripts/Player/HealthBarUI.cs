using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider;
    private Health playerHealth;
    private Image fillImage;

    void Start()
    {
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        if (healthSlider != null && healthSlider.fillRect != null)
        {
            fillImage = healthSlider.fillRect.GetComponent<Image>();
        }
    }

    void Update()
    {
        if (playerHealth == null)
        {
            FindPlayer();
        }
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<Health>();

            if (playerHealth != null)
            {
                healthSlider.maxValue = playerHealth.maxHealth;

                playerHealth.OnDamage.AddListener(UpdateHealthBar);
                playerHealth.OnHeal.AddListener(UpdateHealthBar);

                UpdateHealthBar();
            }
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
        if (fillImage != null && playerHealth != null)
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