using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Upgrade Values")]
    public float damageMultiplier = 1.2f;
    public float speedMultiplier = 1.2f;
    public int healthBonus = 20;

    [Header("UI Panel")]
    public GameObject upgradePanel;

    private bool waitingForUpgrade = false;

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
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }

    public void ResetUpgrades()
    {
        // Здесь можно сбросить улучшения, если нужно
        // Например, удалить все бонусы с игрока при новом забеге
        Debug.Log("Все улучшения сброшены для нового забега");
    }

    public void ShowUpgradeMenu()
    {
        waitingForUpgrade = true;
        Time.timeScale = 0f;

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
        }

        Debug.Log("=== ВЫБЕРИ УЛУЧШЕНИЕ ===");
        Debug.Log("Нажми 1 - Урон +20%");
        Debug.Log("Нажми 2 - Скорость +20%");
        Debug.Log("Нажми 3 - Здоровье +20");
    }

    void Update()
    {
        if (!waitingForUpgrade) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ApplyUpgrade("damage");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ApplyUpgrade("speed");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ApplyUpgrade("health");
        }
    }

    void ApplyUpgrade(string type)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            waitingForUpgrade = false;
            Time.timeScale = 1f;
            GameManager.Instance?.ContinueAfterUpgrade();
            return;
        }

        switch (type)
        {
            case "damage":
                PlayerShooting shooting = player.GetComponent<PlayerShooting>();
                if (shooting != null)
                {
                    shooting.damageMultiplier *= damageMultiplier;
                    Debug.Log($"?? Урон увеличен! Теперь: x{shooting.damageMultiplier}");
                }
                break;

            case "speed":
                PlayerMovement movement = player.GetComponent<PlayerMovement>();
                if (movement != null)
                {
                    movement.moveSpeed *= speedMultiplier;
                    Debug.Log($"? Скорость увеличена! Теперь: {movement.moveSpeed}");
                }
                break;

            case "health":
                Health health = player.GetComponent<Health>();
                if (health != null)
                {
                    health.currentHealth += healthBonus;
                    health.maxHealth += healthBonus;
                    Debug.Log($"?? Здоровье увеличено! Теперь: {health.currentHealth}/{health.maxHealth}");
                }
                break;
        }

        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        waitingForUpgrade = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ContinueAfterUpgrade();
        }
        else
        {
            Time.timeScale = 1f;
            Debug.LogError("GameManager.Instance is null!");
        }
    }
}