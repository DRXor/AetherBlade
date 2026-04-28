using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePortal : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Имя сцены главного меню
    [SerializeField] private float delayBeforeExit = 1f; // Задержка перед выходом

    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            Debug.Log("EndGamePortal: Player entered final portal! Game completed!");

            // Запускаем переход в главное меню с задержкой
            Invoke(nameof(ReturnToMainMenu), delayBeforeExit);
        }
    }

    private void ReturnToMainMenu()
    {
        Debug.Log("EndGamePortal: Loading main menu...");
        SceneManager.LoadScene(mainMenuSceneName);
    }
}