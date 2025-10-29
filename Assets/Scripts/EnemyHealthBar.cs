using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private HealthEnemy enemyHealth;

    void Start()
    {
        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
        }

        if (enemyHealth == null)
        {
            enemyHealth = GetComponentInParent<HealthEnemy>();
        }

        if (enemyHealth != null && healthSlider != null)
        {
            enemyHealth.EnemyDamage.AddListener(UpdateHealthBar);

            StartCoroutine(InitializeAfterDelay());
        }
        else
        {
            Debug.LogError("EnemyHealthBar: Не удалось найти HealthEnemy или Slider!", this);
        }
    }

    System.Collections.IEnumerator InitializeAfterDelay()
    {
        yield return new WaitForEndOfFrame();

        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (enemyHealth == null || healthSlider == null) return;

        float healthPercent = enemyHealth.current_health / enemyHealth.max_health;
        healthSlider.value = healthPercent;

        Debug.Log($"Health Bar Updated: {enemyHealth.current_health}/{enemyHealth.max_health} = {healthPercent}");
    }

    void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.EnemyDamage.RemoveListener(UpdateHealthBar);
        }
    }
}