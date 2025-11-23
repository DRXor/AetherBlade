using UnityEngine;

public class ArtifactPickup : MonoBehaviour
{
    [Header("Artifact Settings")]
    public GameObject pickupEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory.Instance.CollectArtifact();

            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, transform.rotation);

            Destroy(gameObject);

            Debug.Log("Артефакт подобран!");
        }
    }
}