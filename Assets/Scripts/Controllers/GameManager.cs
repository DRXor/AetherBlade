using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton для доступа из других скриптов

    [Header("Настройки сцен")]
    public string mainMenuScene = "MainMenu";
    public string firstLevelScene = "FirstAct";

    [Header("Настройки управления")]
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("UI")]
    public GameObject gameOverPanel;
    public GameObject pauseMenuPanel;

    [Header("Состояние игры")]
    public bool isGameOver = false;
    public bool isPaused = false;

    void Awake()
    {
        // Singleton паттерн - чтобы GameManager был один
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Не уничтожать между сценами
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Находим UI элементы на сцене (если не назначены в инспекторе)
        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");
        if (pauseMenuPanel == null)
            pauseMenuPanel = GameObject.Find("PauseMenuPanel");

        // Скрываем панели при старте
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        // Обработка паузы (только если игра не окончена)
        if (!isGameOver && Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    // ========== МЕТОДЫ СМЕРТИ ==========

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        isPaused = false; // Снимаем паузу, если была

        Debug.Log("GAME OVER!");

        // Останавливаем время
        Time.timeScale = 0f;

        // Показываем панель Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

        }
        else
        {
            Debug.LogWarning("GameOverPanel не назначен в GameManager!");
        }

        // Отключаем управление игроком
        DisablePlayerControl();
    }

    void DisablePlayerControl()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Отключаем все компоненты, которые могут управлять игроком
            var scripts = player.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                // Не отключаем сам Health, чтобы не сломать логику
                if (script.GetType() != typeof(Health))
                    script.enabled = false;
            }
        }
    }

    // ========== МЕТОДЫ ПАУЗЫ ==========

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (isGameOver) return; // Нельзя паузу, если игра окончена

        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Debug.Log("Игра на паузе");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Debug.Log("Игра продолжена");
    }

    // ========== МЕТОДЫ ЗАГРУЗКИ СЦЕН ==========

    public void RestartLevel()
    {
        Debug.Log("=== RESTART BUTTON CLICKED ==="); // <-- добавить

        Debug.Log("Перезапуск уровня...");

        // Скрываем панель Game Over ДО перезагрузки
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Сбрасываем выделение, чтобы не было "залипшей" кнопки
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        Time.timeScale = 1f;
        isGameOver = false;
        isPaused = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Возвращаемся в главное меню...");

        // Проверяем, есть ли сцена в билде
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (scenePath.Contains(mainMenuScene))
            {
                sceneExists = true;
                break;
            }
        }

        if (!sceneExists)
        {
            Debug.LogError("Сцена " + mainMenuScene + " не найдена в Build Settings!");
            return;
        }

        // Сбрасываем выделение
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        // Сбрасываем состояние
        Time.timeScale = 1f;
        isGameOver = false;
        isPaused = false;

        // Загружаем главное меню
        SceneManager.LoadScene(mainMenuScene);
    }

    public void LoadLevel(string levelName)
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isPaused = false;
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Для тестирования в редакторе
#endif
    }

    // ========== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ==========

    // Вызывается при загрузке новой сцены
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // После загрузки сцены сбрасываем состояние
        Time.timeScale = 1f;
        isGameOver = false;
        isPaused = false;

        //Ищем панели заново на новой сцене
        gameOverPanel = GameObject.Find("GameOverPanel");
        pauseMenuPanel = GameObject.Find("PauseMenuPanel");

        // Скрываем панели
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);

            // НАЙТИ КНОПКИ И ПЕРЕНАЗНАЧИТЬ
            var buttons = gameOverPanel.GetComponentsInChildren<UnityEngine.UI.Button>();

            foreach (var btn in buttons)
            {
                if (btn.name == "RestartButton")
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(RestartLevel);
                }

                if (btn.name == "MenuButton")
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(ReturnToMainMenu);
                }
            }
        }
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Ищем игрока на новой сцене
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("Игрок найден на сцене: " + player.name);
        }
        else
        {
            Debug.LogWarning("Игрок не найден на сцене " + scene.name);
        }
    }
}