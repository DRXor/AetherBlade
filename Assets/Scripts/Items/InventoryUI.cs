using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public Text artifactsText;
    public Text coinsText;
    public Image shieldIcon;
    public Image weaponIcon;

    void Start()
    {
        Inventory.Instance.OnArtifactsChanged += UpdateArtifactsUI;
        Inventory.Instance.OnCoinsChanged += UpdateCoinsUI;
        Inventory.Instance.OnShieldChanged += UpdateShieldUI;
        Inventory.Instance.OnWeaponChanged += UpdateWeaponUI;

        UpdateArtifactsUI(Inventory.Instance.artifactsCollected);
        UpdateCoinsUI(Inventory.Instance.coinsCollected);
        UpdateShieldUI(Inventory.Instance.hasShield);
        UpdateWeaponUI(Inventory.Instance.hasWeapon);
    }

    void UpdateArtifactsUI(int count)
    {
        if (artifactsText != null)
            artifactsText.text = $"Артефакты: {count}";
    }

    void UpdateCoinsUI(int count)
    {
        if (coinsText != null)
            coinsText.text = $"Монеты: {count}";
    }

    void UpdateShieldUI(bool hasShield)
    {
        if (shieldIcon != null)
        {
            shieldIcon.color = hasShield ? Color.white : Color.gray;
            shieldIcon.transform.localScale = hasShield ? Vector3.one : Vector3.one * 0.8f;
        }
    }

    void UpdateWeaponUI(bool hasWeapon)
    {
        if (weaponIcon != null)
        {
            weaponIcon.color = hasWeapon ? Color.white : Color.gray;
            weaponIcon.transform.localScale = hasWeapon ? Vector3.one : Vector3.one * 0.8f;
        }
    }

    void OnDestroy()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.OnArtifactsChanged -= UpdateArtifactsUI;
            Inventory.Instance.OnCoinsChanged -= UpdateCoinsUI;
            Inventory.Instance.OnShieldChanged -= UpdateShieldUI;
            Inventory.Instance.OnWeaponChanged -= UpdateWeaponUI;
        }
    }
}