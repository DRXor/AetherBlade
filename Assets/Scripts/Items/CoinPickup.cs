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
            Inventory.Instance.CollectCoins(coinValue);

            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, transform.rotation);

            Destroy(gameObject);

            Debug.Log($"Монета подобрана! +{coinValue}");
        }
    }
}