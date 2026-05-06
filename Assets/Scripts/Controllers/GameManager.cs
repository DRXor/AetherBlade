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
    private int currentLevelIndex = -1;
    private int score = 0;

    private GameObject currentPlayer = null; // Сохраняем ссылку на игрока

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
        currentLevelIndex = -1;
        LoadRandomLevel();
    }

    public void LoadRandomLevel()
    {
        if (isGameOver) return;

        int randomIndex = Random.Range(0, availableLevels.Count);
        int sceneToLoad = availableLevels[randomIndex];

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);

        Debug.Log($"Loading level: {sceneToLoad}");
    }

    public void CompleteLevel()
    {
        if (isGameOver) return;

        score++;
        Debug.Log($"Level complete! Score: {score} - Loading next level...");

        Time.timeScale = 1f;
        LoadRandomLevel();
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

            Text[] texts = gameOverPanel.GetComponentsInChildren<Text>();
            foreach (Text t in texts)
            {
                if (t.name == "ScoreText" || t.name == "WavesText")
                {
                    t.text = $"Waves Completed: {score}";
                }
                else if (t.name == "TitleText")
                {
                    t.text = "GAME OVER";
                }
            }

            Button[] buttons = gameOverPanel.GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
            {
                if (btn.name == "RestartButton")
                    btn.onClick.AddListener(RestartRun);
                else if (btn.name == "MenuButton")
                    btn.onClick.AddListener(ReturnToMainMenu);
            }
        }
        else
        {
            Debug.LogError("GameOverPanel is NULL!");
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
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        if (currentPlayer != null)
        {
            Health health = currentPlayer.GetComponent<Health>();
            if (health != null)
            {
                health.currentHealth = health.maxHealth;
                health.currentShield = 0f;
                health.UpdateUI();
            }
        }

        LoadRandomLevel();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        isGameOver = false;

        // Удаляем игрока перед возвратом в меню
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
        }

        SceneManager.LoadScene(0);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        isPaused = false;

        // ПОИСК ПАНЕЛЕЙ
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

        // ========== ГЛАВНОЕ МЕНЮ (сцена 0) ==========
        if (scene.buildIndex == 0)
        {
            // Удаляем игрока если он есть
            if (currentPlayer != null)
            {
                Destroy(currentPlayer);
                currentPlayer = null;
            }

            // Также ищем и удаляем любого игрока в сцене
            GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
            if (existingPlayer != null)
            {
                Destroy(existingPlayer);
            }

            Debug.Log("Main menu - Player destroyed");
            return; // Выходим, дальше не идём
        }

        // ========== ИГРОВЫЕ СЦЕНЫ (1, 2, 3...) ==========

        // Ищем существующего игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Если игрока нет в сцене - создаём
        if (player == null)
        {
            GameData data = FindFirstObjectByType<GameData>();
            if (data != null && data.playerPrefab != null)
            {
                player = Instantiate(data.playerPrefab, Vector3.zero, Quaternion.identity);
                player.tag = "Player";
                currentPlayer = player;
                Debug.Log("New player CREATED in game scene!");
            }
            else
            {
                Debug.LogError("GameData or playerPrefab is null!");
                return;
            }
        }
        else
        {
            currentPlayer = player;
        }

        // Включаем игрока
        if (player != null)
        {
            player.SetActive(true);

            // Телепортируем к спавну
            GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                Debug.Log($"Player teleported to spawn: {spawnPoint.transform.position}");
            }

            // Сбрасываем здоровье
            Health health = player.GetComponent<Health>();
            if (health != null)
            {
                health.currentHealth = health.maxHealth;
                health.currentShield = 0f;
                health.UpdateUI();
                Debug.Log($"Health reset to {health.currentHealth}/{health.maxHealth}");
            }
        }

        Debug.Log($"Game scene loaded: {scene.buildIndex}");
    }

    void Update()
    {
        if (!isGameOver && Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }

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