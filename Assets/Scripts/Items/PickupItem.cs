using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public AudioClip pickupSound; // Можно перетащить звук в инспекторе
    public int coinValue = 1;
    public float healAmount = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Звук подбора
            if (pickupSound != null && AudioManager.instance != null)
            {
                AudioManager.instance.PlaySound(pickupSound);
            }
            else if (AudioManager.instance != null && AudioManager.instance.pickupSound != null)
            {
                // Используем стандартный звук, если свой не задан
                AudioManager.instance.PlaySound(AudioManager.instance.pickupSound);
            }

            // Награда
            if (coinValue > 0)
            {
                Inventory.Instance.CollectCoins(coinValue);
            }

            if (healAmount > 0)
            {
                Health playerHealth = other.GetComponent<Health>();
                if (playerHealth != null)
                    playerHealth.Heal(healAmount);
            }

            Destroy(gameObject);
        }
    }
}
