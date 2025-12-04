using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthDebug : MonoBehaviour
{
    [Header("UI Reference")]
    public Slider healthSlider; // Если есть полоска здоровья у врага
    public Text healthText; // Если есть текст здоровья

    private HealthEnemy healthSystem;

    void Start()
    {
        healthSystem = GetComponent<HealthEnemy>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = healthSystem.max_health;
            healthSlider.value = healthSystem.current_health;
        }
    }

    void Update()
    {
        // Логируем здоровье каждую секунду
        if (Time.frameCount % 60 == 0) // Каждую секунду при 60 FPS
        {
            Debug.Log($"Enemy {name} health: {healthSystem.current_health}/{healthSystem.max_health}");
        }

        // Обновляем UI если есть
        if (healthSlider != null)
        {
            healthSlider.value = healthSystem.current_health;
        }

        if (healthText != null)
        {
            healthText.text = $"HP: {healthSystem.current_health}/{healthSystem.max_health}";
        }
    }
}
