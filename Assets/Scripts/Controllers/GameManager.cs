using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic; 

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

    [Header("Level Order")]
    private List<int> randomLevelOrder;
    private int currentLevelProgress = 0;

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

    public void StartRandomGame()
    {
        randomLevelOrder = new List<int> { 1, 2, 3 };

        for (int i = 0; i < randomLevelOrder.Count; i++)
        {
            int temp = randomLevelOrder[i];
            int randomIndex = Random.Range(i, randomLevelOrder.Count);
            randomLevelOrder[i] = randomLevelOrder[randomIndex];
            randomLevelOrder[randomIndex] = temp;
        }

        currentLevelProgress = 0;
        LoadNextLevel(); 
    }

    public void LoadNextLevel()
    {
        if (randomLevelOrder != null && currentLevelProgress < randomLevelOrder.Count)
        {
            int nextSceneIndex = randomLevelOrder[currentLevelProgress];
            currentLevelProgress++;

            Debug.Log($"Загружаем случайный уровень с индексом: {nextSceneIndex}");
            ResetGameState();
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Уровни закончились! Возврат в меню.");
            ReturnToMainMenu();
        }
    }

    public void CompleteLevel()
    {
        Time.timeScale = 0f;

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);

            Transform current = levelCompletePanel.transform;
            while (current.parent != null)
            {
                current = current.parent;
                current.gameObject.SetActive(true);
            }

            Debug.Log("Уровень пройден! Мы принудительно включили всю иерархию до самого верха.");

            Transform nextBtn = levelCompletePanel.transform.Find("NextLevelButton");
            Transform menuBtn = levelCompletePanel.transform.Find("BackToMainMenu");

            bool isLastLevel = (randomLevelOrder != null && currentLevelProgress >= randomLevelOrder.Count);

            if (isLastLevel)
            {
                if (nextBtn != null) nextBtn.gameObject.SetActive(false);
                if (menuBtn != null) menuBtn.gameObject.SetActive(true);
            }
            else
            {
                if (nextBtn != null) nextBtn.gameObject.SetActive(true);
            }
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        isPaused = false;
        Debug.Log("GAME OVER!");
        Time.timeScale = 0f;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

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
                if (script != this && script.enabled) script.enabled = false;
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
        currentLevelProgress = 0;
        randomLevelOrder = null;  
        SceneManager.LoadScene(0);
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGameState();

        gameOverPanel = FindGameObjectAnywhere("GameOverPanel");
        pauseMenuPanel = FindGameObjectAnywhere("PauseMenuPanel");
        levelCompletePanel = FindGameObjectAnywhere("LevelCompletePanel");

        if (levelCompletePanel == null)
            Debug.LogError($"!!! GameManager не нашел 'LevelCompletePanel' на сцене {scene.name}. Проверь имя в иерархии!");

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (levelCompletePanel) levelCompletePanel.SetActive(false);

        SetupButtons();
    }

    private GameObject FindGameObjectAnywhere(string name)
    {
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.name == name && obj.scene.isLoaded)
            {
                return obj;
            }
        }
        return null;
    }

    void SetupButtons()
    {
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();

        foreach (var btn in allButtons)
        {
            if (!btn.gameObject.scene.isLoaded) continue;

            btn.onClick.RemoveAllListeners();

            if (btn.name == "ResumeButton") btn.onClick.AddListener(ResumeGame);
            else if (btn.name == "NextLevelButton") btn.onClick.AddListener(LoadNextLevel);
            else if (btn.name == "RestartButton") btn.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
            else if (btn.name == "MenuButton" || btn.name == "BackToMainMenu") btn.onClick.AddListener(ReturnToMainMenu);
        }
    }

    void Update()
    {
        if (!isGameOver && Input.GetKeyDown(pauseKey)) TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        if (isGameOver) return;
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