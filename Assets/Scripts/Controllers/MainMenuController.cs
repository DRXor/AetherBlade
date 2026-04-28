using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Кнопки меню")]
    public Button startButton;
    public Button quitButton;

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        Debug.Log("Генерируем случайные уровни и запускаем игру...");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartRandomGame();
        }
        else
        {
            Debug.LogError("GameManager не найден на сцене!");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}