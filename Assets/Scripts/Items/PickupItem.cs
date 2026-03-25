using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.PlaySound(pickupSound);
            Destroy(gameObject);
        }
    }
}
