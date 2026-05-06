using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;
    public GameObject pickupEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ========== ДОБАВЛЯЕМ ЗВУК ==========
            if (AudioManager.instance != null && AudioManager.instance.pickupSound != null)
            {
                AudioManager.instance.PlaySound(AudioManager.instance.pickupSound);
            }
            else
            {
                Debug.LogWarning("AudioManager or pickupSound is missing!");
            }

            Inventory.Instance.CollectCoins(coinValue);

            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, transform.rotation);

            Destroy(gameObject);

            Debug.Log($"Монета подобрана! +{coinValue}");
        }
    }
}