using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
    }

    void Update()
    {
        // ТОЛЬКО ESC, убираем пробел
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        // Останавливаем звуки шагов
        AudioListener.pause = true;
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        AudioListener.pause = false;
    }

    public void Button_Resume()
    {
        Resume();
    }

    public void Button_MainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        isPaused = false;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Возвращаемся в главное меню
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        Debug.Log("Returning to main menu from pause!");
    }
}