using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject pauseMenuPanel;
    public GameObject levelCompletePanel;

    [Header("Upgrade")]
    public UpgradeUI upgradeUI;

    [Header("State")]
    public bool isGameOver = false;
    public bool isPaused = false;

    [Header("Controls")]
    public KeyCode pauseKey = KeyCode.Escape; 

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
            return;
        }
    }

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Loading Level Index: {nextSceneIndex}");
            ResetGameState();
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("End of the game reached! Returning to Menu.");
            ReturnToMainMenu();
        }
    }

    public void CompleteLevel()
    {
        Debug.Log("Level Complete → showing upgrades");

        if (upgradeUI != null)
        {
            upgradeUI.Show();
        }
        else
        {
            Debug.LogWarning("UpgradeUI not assigned!");
        }
    }
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        isPaused = false;

        Debug.Log("GAME OVER! Player has died.");

        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameManager: gameOverPanel is not assigned!");
        }

        Time.timeScale = 0f;
    }


    public void ResetGameState()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isPaused = false;
    }

    public void ReturnToMainMenu()
    {
        ResetGameState();
        SceneManager.LoadScene(0);
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGameState();

        gameOverPanel = GameObject.Find("GameOverPanel");
        pauseMenuPanel = GameObject.Find("PauseMenuPanel");
        levelCompletePanel = GameObject.Find("LevelCompletePanel");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (levelCompletePanel) levelCompletePanel.SetActive(false);

        SetupButtons();
    }

    void SetupButtons()
    {
        GameObject canvas = GameObject.Find("Canvas_Main");
        if (canvas == null) return;

        Button[] allButtons = canvas.GetComponentsInChildren<Button>(true);

        foreach (var btn in allButtons)
        {
            btn.onClick.RemoveAllListeners();

            if (btn.name == "ResumeButton")
            {
                btn.onClick.AddListener(ResumeGame);
                Debug.Log("Connected Resume button");
            }
            else if (btn.name == "NextLevelButton")
            {
                btn.onClick.AddListener(LoadNextLevel);
                Debug.Log("Connected Next Level button");
            }
            else if (btn.name == "RestartButton")
            {
                btn.onClick.AddListener(RestartRun);
                Debug.Log("Connected Restart button");
            }
            else if (btn.name == "MenuButton" || btn.name == "BackToMainMenu")
            {
                btn.onClick.AddListener(ReturnToMainMenu);
                Debug.Log("Connected Menu button");
            }
        }
    }

    void Update()
    {
        if (!isGameOver && Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (isGameOver) return;

        isPaused = true;
        Time.timeScale = 0f; 

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; 

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Debug.Log("Game Resumed");
    }

    public void RestartRun()
    {
        Debug.Log("Restarting run...");

        Time.timeScale = 1f;
        ResetGameState();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}