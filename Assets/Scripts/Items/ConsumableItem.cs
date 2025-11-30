using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class ConsumableItem : Item
{
    [Header("Consumable Effects")]
    public float healthRestore = 0f;
    public float shieldRestore = 0f;
    public float effectDuration = 0f;

    void OnEnable()
    {
        itemType = ItemType.Consumable;
    }
}