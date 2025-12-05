using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text artifactsText;
    public TMP_Text coinsText;
    public TMP_Text keysText;

    void Start()
    {
        Inventory.Instance.OnArtifactsChanged += UpdateArtifactsUI;
        Inventory.Instance.OnCoinsChanged += UpdateCoinsUI;
        Inventory.Instance.OnKeysChanged += UpdateKeysUI;

        UpdateArtifactsUI(Inventory.Instance.artifactsCollected);
        UpdateCoinsUI(Inventory.Instance.coinsCollected);
        UpdateKeysUI(Inventory.Instance.keysCollected);
    }

    void UpdateArtifactsUI(int count)
    {
        if (artifactsText != null)
            artifactsText.text = $"Арт: {count}";
    }

    void UpdateCoinsUI(int count)
    {
        if (coinsText != null)
            coinsText.text = $"Мон: {count}";
    }

    void UpdateKeysUI(int count)
    {
        if (keysText != null)
            keysText.text = $"Кл: {count}";
    }

    void OnDestroy()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.OnArtifactsChanged -= UpdateArtifactsUI;
            Inventory.Instance.OnCoinsChanged -= UpdateCoinsUI;
            Inventory.Instance.OnKeysChanged -= UpdateKeysUI;
        }
    }
}