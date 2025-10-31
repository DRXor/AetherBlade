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
        // F - ������������/�������������� ���
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (playerShield != null)
            {
                if (playerShield.isShieldActive)
                {
                    playerShield.DeactivateShield();
                }
                else
                {
                    playerShield.ActivateShield();
                }
                LogStatus();
            }
        }

    }

    void LogStatus()
    {
        if (playerShield != null && playerHealth != null)
        {
            string shieldStatus = playerShield.hasShieldItem ?
                $"���: {playerShield.currentShield}/50 ({(playerShield.isShieldActive ? "�������" : "���������")})" :
                "���� ���";
            Debug.Log($"��������: {playerHealth.currentHealth}/100 | {shieldStatus}");
        }
    }
}