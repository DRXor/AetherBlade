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
            if (playerShield != null && !playerShield.hasShieldItem)
            {
                playerShield.PickupShield(shieldAmount);

                if (pickupEffect != null)
                    Instantiate(pickupEffect, transform.position, transform.rotation);

                Destroy(gameObject);

                Debug.Log("Щит подобран!");
            }
            else if (playerShield != null && playerShield.hasShieldItem)
            {
                Debug.Log("У вас уже есть щит!");
            }
        }
    }
}