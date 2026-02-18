using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Настройки")]
    public string mainMenuScene = "MainMenu";
    public KeyCode pauseKey = KeyCode.Escape;

    public void ReturnToMainMenu()
    {
        Debug.Log("Возвращаемся в главное меню...");

        SceneManager.LoadScene(mainMenuScene);
    }
}