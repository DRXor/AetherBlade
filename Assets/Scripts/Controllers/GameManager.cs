using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameOverPanel; // ПЕРЕТАЩИ сюда панель GameOver ВРУЧНУЮ!
    public GameObject pauseMenuPanel;
    public GameObject levelCompletePanel;

    public bool isGameOver = false;
    public bool isPaused = false;
    public KeyCode pauseKey = KeyCode.Escape;

    private List<int> availableLevels = new List<int> { 1, 2, 3 };
    private int currentLevelIndex = -1;
    private int score = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Подписываемся на загрузку сцен
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StartRandomGame()
    {
        score = 0;
        isGameOver = false;
        currentLevelIndex = -1;
        LoadRandomLevel();
    }

    public void LoadRandomLevel()
    {
        if (isGameOver) return;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, availableLevels.Count);
        }
        while (newIndex == currentLevelIndex && availableLevels.Count > 1);

        currentLevelIndex = newIndex;
        int sceneToLoad = availableLevels[currentLevelIndex];

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void CompleteLevel()
    {
        if (isGameOver) return;

        score++;
        Debug.Log($"Level complete! Score: {score}");

        Time.timeScale = 0f;

        // Показываем меню улучшений
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.ShowUpgradeMenu();
        }
        else
        {
            LoadRandomLevel();
        }
    }

    public void ContinueAfterUpgrade()
    {
        Time.timeScale = 1f;
        LoadRandomLevel();
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            Text text = gameOverPanel.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = $"GAME OVER!\nWaves survived: {score}";
            }
        }
        else
        {
            Debug.LogError("GameOverPanel is NULL! Assign it in Inspector!");
        }
    }

    public void RestartRun()
    {
        isGameOver = false;
        score = 0;
        currentLevelIndex = -1;
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        LoadRandomLevel();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene(0);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        isPaused = false;

        // НЕ СОЗДАЁМ ПАНЕЛИ - просто находим их
        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");
        if (pauseMenuPanel == null)
            pauseMenuPanel = GameObject.Find("PauseMenuPanel");
        if (levelCompletePanel == null)
            levelCompletePanel = GameObject.Find("LevelCompletePanel");

        // Выключаем панели
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
    }

    void Update()
    {
        if (!isGameOver && Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }

        // Быстрый рестарт при GameOver по клавише R
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartRun();
        }
    }

    public void TogglePause()
    {
        if (isGameOver) return;

        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }
}