using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    [Header("Shield Pickup Settings")]
    public float shieldAmount = 50f;
    public GameObject pickupEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Shield playerShield = other.GetComponent<Shield>();

            if (playerShield != null)
            {
                playerShield.PickupShield(shieldAmount);

                if (Inventory.Instance != null)
                {
                    Inventory.Instance.PickupShield();
                }

                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, transform.rotation);
                }

                Destroy(gameObject);
            }
        }
    }
}