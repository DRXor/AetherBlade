using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text coinsText;

    void Start()
    {
        Inventory.Instance.OnCoinsChanged += UpdateCoinsUI;

        UpdateCoinsUI(Inventory.Instance.coinsCollected);
    }
    void UpdateCoinsUI(int count)
    {
        if (coinsText != null)
            coinsText.text = $"{count}";
    }
    void OnDestroy()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.OnCoinsChanged -= UpdateCoinsUI;
        }
    }
}