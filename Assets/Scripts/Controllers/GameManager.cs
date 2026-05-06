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
        Debug.Log($"Level complete! Score: {score} - Loading next level...");

        // Прямая загрузка следующего уровня
        Time.timeScale = 1f;
        LoadRandomLevel();
    }

    public void ContinueAfterUpgrade()
    {
        Time.timeScale = 1f;
        LoadRandomLevel(); // Загружаем следующий случайный уровень
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // Находим все тексты на панели и обновляем
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

            // Находим кнопки и вешаем события
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

        // Не удаляем игрока, а просто сбрасываем
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health health = player.GetComponent<Health>();
            if (health != null)
            {
                health.currentHealth = health.maxHealth;
                health.currentShield = 0f;
                health.UpdateUI();
            }

            // Перемещаем на спавн
            GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
            }
        }

        // Просто перезагружаем текущую сцену или новую
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

        // ========== СОЗДАЁМ ИГРОКА ЕСЛИ ЕГО НЕТ ==========
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            GameData data = FindFirstObjectByType<GameData>();
            if (data != null && data.playerPrefab != null)
            {
                // Создаём нового игрока
                player = Instantiate(data.playerPrefab, Vector3.zero, Quaternion.identity);
                player.tag = "Player";
                Debug.Log("New player created!");
            }
            else
            {
                Debug.LogError("GameData or playerPrefab is null!");
            }
        }

        // ========== ТЕЛЕПОРТАЦИЯ К СПАВНУ ==========
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");

        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
            Debug.Log($"Player teleported to spawn: {spawnPoint.transform.position}");
        }
        else if (player != null)
        {
            player.transform.position = Vector3.zero;
            Debug.Log("No spawn point found, teleported to (0,0)");
        }

        // ========== СБРОС ЗДОРОВЬЯ ==========
        Health health = player?.GetComponent<Health>();
        if (health != null)
        {
            health.currentHealth = health.maxHealth;
            health.currentShield = 0f;
            health.UpdateUI();
            Debug.Log($"Health reset to {health.currentHealth}/{health.maxHealth}");
        }
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