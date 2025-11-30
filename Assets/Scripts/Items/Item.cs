using UnityEngine;

public abstract class Item : ScriptableObject
{
    [Header("Basic Item Settings")]
    public string itemName;
    public string description;
    public Sprite icon;
    public ItemType itemType;
    public int maxStack = 1;
}

public enum ItemType
{
    Artifact,    // Сюжетные артефакты
    Consumable,  // Зелья, аптечки
    Weapon,      // Оружие
    Shield,      // Щиты
    Currency,    // Монеты
    Key          // Ключи для сюжета
}