using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Portal: Player entered! Notifying GameManager...");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteLevel();
            }
            else
            {
                Debug.LogError("Portal: GameManager Instance not found!");
            }
        }
    }
}