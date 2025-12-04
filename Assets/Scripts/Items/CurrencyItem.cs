using UnityEngine;

[CreateAssetMenu(fileName = "New Currency", menuName = "Inventory/Currency")]
public class CurrencyItem : Item
{
    [Header("Currency Settings")]
    public int value = 1;

    void OnEnable()
    {
        itemType = ItemType.Currency;
        maxStack = 999;
    }
}