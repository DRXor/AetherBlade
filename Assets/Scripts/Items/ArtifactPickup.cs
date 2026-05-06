using UnityEngine;

public class ArtifactPickup : MonoBehaviour
{
    [Header("Artifact Settings")]
    public GameObject pickupEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ========== ÄÎÁÀÂËßÅÌ ÇÂÓÊ ==========
            if (AudioManager.instance != null && AudioManager.instance.pickupSound != null)
            {
                AudioManager.instance.PlaySound(AudioManager.instance.pickupSound);
            }

            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, transform.rotation);

            Destroy(gameObject);

            Debug.Log("Àğòåôàêò ñîáğàí!");
        }
    }
}