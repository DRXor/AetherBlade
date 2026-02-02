using UnityEngine;

public class ConsumablePickup : MonoBehaviour
{
    [Header("Consumable Pickup Settings")]
    public float healAmount = 30f;  // —колько здоровь€ восстанавливает
    public GameObject pickupEffect; // Ёффект при подборе (опционально)

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                // ¬осстанавливаем здоровье
                playerHealth.Heal((int)healAmount);

                // Ёффект при подборе
                if (pickupEffect != null)
                    Instantiate(pickupEffect, transform.position, transform.rotation);

                // ”ничтожаем колбочку
                Destroy(gameObject);

                Debug.Log($"ѕодобрано здоровье: +{healAmount} HP");
            }
        }
    }
}