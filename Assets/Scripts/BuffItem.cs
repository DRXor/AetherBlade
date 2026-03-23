using UnityEngine;

public class BuffItem : MonoBehaviour
{
    [Header("Тип баффа")]
    public BuffType buffType = BuffType.DamageBoost;

    [Header("Настройки")]
    public float duration = 10f;
    public float damageMultiplier = 2f;
    public float speedMultiplier = 2f;

    [Header("Визуал (опционально)")]
    public GameObject pickupEffect;

    [Header("UI")]
    public Sprite buffIcon;

    public enum BuffType
    {
        DamageBoost,
        SpeedBoost,
        Invincibility
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

            switch (buffType)
            {
                case BuffType.DamageBoost:
                    if (playerHealth != null)
                        playerHealth.ApplyDamageBuff(damageMultiplier, duration);
                    break;

                case BuffType.SpeedBoost:
                    if (playerMovement != null)
                        playerMovement.ApplySpeedBuff(speedMultiplier, duration);
                    break;

                case BuffType.Invincibility:
                    if (playerHealth != null)
                        playerHealth.ApplyInvincibility(duration);
                    break;
            }

            Debug.Log($"Подобран бафф: {buffType} на {duration} сек");
            BuffUI ui = FindObjectOfType<BuffUI>();
            Debug.Log("UI найден: " + ui);
            Debug.Log("Иконка: " + buffIcon);

            if (ui != null)
            {
                ui.ShowBuff(buffIcon, duration);
            }
            Destroy(gameObject);
        }
    }
}
