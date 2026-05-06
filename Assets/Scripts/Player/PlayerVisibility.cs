using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVisibility : MonoBehaviour
{
    private void Start()
    {
        // Подписываемся на загрузку сцен
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Проверяем текущую сцену
        CheckVisibility(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckVisibility(scene);
    }

    private void CheckVisibility(Scene scene)
    {
        if (scene.buildIndex == 0) // Главное меню
        {
            // Делаем невидимым, но НЕ отключаем полностью
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            // Отключаем коллайдер
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // Отключаем управление
            PlayerMovement movement = GetComponent<PlayerMovement>();
            if (movement != null) movement.enabled = false;

            // Отключаем стрельбу
            PlayerShooting shooting = GetComponent<PlayerShooting>();
            if (shooting != null) shooting.enabled = false;

            Debug.Log("Player hidden in main menu");
        }
        else // Игровые сцены
        {
            // Делаем видимым
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = true;

            // Включаем коллайдер
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = true;

            // Включаем управление
            PlayerMovement movement = GetComponent<PlayerMovement>();
            if (movement != null) movement.enabled = true;

            // Включаем стрельбу
            PlayerShooting shooting = GetComponent<PlayerShooting>();
            if (shooting != null) shooting.enabled = true;

            // Телепортируем к спавну
            GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (spawnPoint != null)
            {
                transform.position = spawnPoint.transform.position;
            }

            // Сбрасываем здоровье
            Health health = GetComponent<Health>();
            if (health != null)
            {
                health.currentHealth = health.maxHealth;
                health.currentShield = 0f;
                health.UpdateUI();
            }

            Debug.Log("Player visible in game scene");
        }
    }
}