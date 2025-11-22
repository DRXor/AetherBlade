using UnityEngine;

public class ShieldTester : MonoBehaviour
{
    private Shield playerShield;
    private Health playerHealth;

    void Start()
    {
        playerShield = GetComponent<Shield>();
        playerHealth = GetComponent<Health>();
    }

    void Update()
    {
        // F - добавить/восстановить щит
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (playerShield != null)
            {
                playerShield.AddShield(50f);
                Debug.Log("🛡️ Щит добавлен! Текущий щит: " + playerShield.currentShield);
                LogStatus();
            }
        }

    }

    void LogStatus()
    {
        if (playerShield != null && playerHealth != null)
        {
            string shieldStatus = playerShield.hasShield ? $"🛡️ Щит: {playerShield.currentShield}/50" : "🛡️ Щита нет";
            Debug.Log($"❤️ Здоровье: {playerHealth.currentHealth}/100 | {shieldStatus}");
        }
    }
}