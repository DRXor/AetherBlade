using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu Settings")]
    public GameObject pauseMenuUI;     // ← Сюда должен быть перетащен PauseCanvas

    private bool isPaused = false;

    void Start()
    {
        Debug.Log("PauseMenu script STARTED");

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            Debug.Log("Pause menu UI was disabled in Start");
        }
        else
        {
            Debug.LogError("pauseMenuUI is NOT assigned! Drag PauseCanvas into this field.");
        }

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pressed!");

            if (pauseMenuUI == null)
            {
                Debug.LogError("pauseMenuUI is NULL! Cannot show menu.");
                return;
            }

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("PAUSE MENU ACTIVATED");
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("PAUSE MENU DEACTIVATED");
    }

    // Для кнопок на UI (Resume)
    public void Button_Resume()
    {
        Resume();
    }

    // Для кнопки Exit (по желанию)
    public void Button_Exit()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}