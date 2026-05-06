using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameOverPanel;
    public GameObject pauseMenuPanel;
    public GameObject levelCompletePanel;

    public bool isGameOver = false;
    public bool isPaused = false;
    public KeyCode pauseKey = KeyCode.Escape;

    private List<int> availableLevels = new List<int> { 1, 2, 3 };
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
        LoadRandomLevel();
    }

    public void LoadRandomLevel()
    {
        if (isGameOver) return;

        int randomIndex = Random.Range(0, availableLevels.Count);
        int sceneToLoad = availableLevels[randomIndex];

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void CompleteLevel()
    {
        if (isGameOver) return;
        score++;
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
            Text[] texts = gameOverPanel.GetComponentsInChildren<Text>();
            foreach (Text t in texts)
            {
                if (t.name == "ScoreText" || t.name == "WavesText")
                    t.text = $"Waves Completed: {score}";
            }
        }
    }

    public void RestartRun()
    {
        isGameOver = false;
        score = 0;
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

        // Поиск панелей (ваш существующий код)
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
        {
            if (gameOverPanel == null)
            {
                Transform found = canvas.transform.Find("GameOverPanel");
                if (found != null) gameOverPanel = found.gameObject;
            }
            if (pauseMenuPanel == null)
            {
                Transform found = canvas.transform.Find("PauseMenuPanel");
                if (found != null) pauseMenuPanel = found.gameObject;
            }
            if (levelCompletePanel == null)
            {
                Transform found = canvas.transform.Find("LevelCompletePanel");
                if (found != null) levelCompletePanel = found.gameObject;
            }
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        // ========== ВОТ ЭТО ДОБАВИТЬ ==========
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (scene.buildIndex == 0) // Главное меню
        {
            if (player != null)
            {
                // Отключаем скрипты стрельбы и движения
                PlayerShooting shooting = player.GetComponent<PlayerShooting>();
                if (shooting != null) shooting.enabled = false;

                PlayerMovement movement = player.GetComponent<PlayerMovement>();
                if (movement != null) movement.enabled = false;
            }
        }
        else // Игровые сцены
        {
            if (player != null)
            {
                // Включаем скрипты обратно
                PlayerShooting shooting = player.GetComponent<PlayerShooting>();
                if (shooting != null) shooting.enabled = true;

                PlayerMovement movement = player.GetComponent<PlayerMovement>();
                if (movement != null) movement.enabled = true;
            }
        }
    }

    public void ContinueAfterUpgrade()
    {
        Time.timeScale = 1f;
        LoadRandomLevel();
    }

    void Update()
    {
        if (!isGameOver && Input.GetKeyDown(pauseKey))
            TogglePause();

        if (isGameOver && Input.GetKeyDown(KeyCode.R))
            RestartRun();
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