using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableInMainMenu : MonoBehaviour
{
    private PlayerShooting shooting;
    private PlayerMovement movement;

    void Start()
    {
        shooting = GetComponent<PlayerShooting>();
        movement = GetComponent<PlayerMovement>();

        SceneManager.sceneLoaded += OnSceneLoaded;
        CheckCurrentScene();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckCurrentScene();
    }

    void CheckCurrentScene()
    {
        bool isMainMenu = (SceneManager.GetActiveScene().buildIndex == 0);

        if (shooting != null)
            shooting.enabled = !isMainMenu;

        if (movement != null)
            movement.enabled = !isMainMenu;
    }
}