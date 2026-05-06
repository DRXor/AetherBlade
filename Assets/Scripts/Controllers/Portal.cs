using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Portal: Player entered! Completing level...");

            if (GameManager.Instance != null && !GameManager.Instance.isGameOver)
            {
                GameManager.Instance.CompleteLevel();
            }
            else
            {
                Debug.LogError("Portal: GameManager Instance not found or game is over!");
            }
        }
    }
}