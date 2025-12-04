using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Кнопки меню")]
    public Button startButton;
    public Button quitButton;

    [Header("Название сцены с игрой")]
    public string gameSceneName = "FirstAct";

    void Start()
    {
        if (startButton == null)
            Debug.LogError("Start Button не назначена!");
        if (quitButton == null)
            Debug.LogError("Quit Button не назначена!");

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        Debug.Log("Загружаем игру...");

        if (SceneUtility.GetBuildIndexByScenePath(gameSceneName) < 0)
        {
            Debug.LogError($"Сцена '{gameSceneName}' не найдена! Добавь её в Build Settings.");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }
}