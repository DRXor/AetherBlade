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
        Time.timeScale = 0f;

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);

            Transform nextBtn = levelCompletePanel.transform.Find("NextLevelButton");
            Transform menuBtn = levelCompletePanel.transform.Find("BackToMainMenu");

            int currentIdx = SceneManager.GetActiveScene().buildIndex;
            bool isLastLevel = (currentIdx + 1 >= SceneManager.sceneCountInBuildSettings);

            if (isLastLevel)
            {
                Debug.Log("Final Level Complete! Showing Menu button only.");

                if (nextBtn != null) nextBtn.gameObject.SetActive(false); 
                if (menuBtn != null) menuBtn.gameObject.SetActive(true); 
            }
            else
            {
                Debug.Log("Level Complete! Showing Next Level button.");

                if (nextBtn != null) nextBtn.gameObject.SetActive(true);  
            }
        }
        else
        {
            Debug.LogWarning("GameManager: levelCompletePanel is not assigned!");
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

        DisablePlayerControl();
    }

    private void DisablePlayerControl()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var scripts = player.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != this && script.enabled)
                    script.enabled = false;
            }
        }
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

        if (gameOverPanel) gameOverPanel.SetActive(false);
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
                btn.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
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
}