using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("Inventory Stats")]
    public int artifactsCollected = 0;
    public int coinsCollected = 0;

    [Header("Active Items")]
    public bool hasShield = false;
    public bool hasWeapon = false;

    private Shield cachedPlayerShield;
    private Health cachedPlayerHealth;

    public event Action<int> OnArtifactsChanged;
    public event Action<int> OnCoinsChanged;
    public event Action<bool> OnShieldChanged;
    public event Action<bool> OnWeaponChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CachePlayerComponents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CachePlayerComponents()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            cachedPlayerShield = player.GetComponent<Shield>();
            cachedPlayerHealth = player.GetComponent<Health>();
        }

        if (cachedPlayerShield == null)
            cachedPlayerShield = FindAnyObjectByType<Shield>();

        if (cachedPlayerHealth == null)
            cachedPlayerHealth = FindAnyObjectByType<Health>();
    }

    public void CollectArtifact()
    {
        artifactsCollected++;
        OnArtifactsChanged?.Invoke(artifactsCollected);
        Debug.Log($"Артефакт собран! Всего: {artifactsCollected}");
    }

    public void CollectCoins(int amount = 1)
    {
        coinsCollected += amount;
        OnCoinsChanged?.Invoke(coinsCollected);
        Debug.Log($"Монеты собраны! Всего: {coinsCollected}");
    }

    public void PickupShield()
    {
        if (!hasShield)
        {
            hasShield = true;
            OnShieldChanged?.Invoke(true);
            Debug.Log("Щит добавлен в инвентарь!");

            if (cachedPlayerShield != null)
            {
                cachedPlayerShield.PickupShield(50f);
            }
            else
            {
                Debug.LogWarning("Компонент Shield не найден на игроке!");
            }
        }
        else
        {
            Debug.Log("Уже есть щит в инвентаре!");
        }
    }

    public void PickupWeapon()
    {
        if (!hasWeapon)
        {
            hasWeapon = true;
            OnWeaponChanged?.Invoke(true);
            Debug.Log("Оружие добавлено в инвентарь!");
        }
    }

    public void UseHealthPotion(float healAmount = 25f)
    {
        if (cachedPlayerHealth != null)
        {
            cachedPlayerHealth.Heal((int)healAmount);
            Debug.Log($"Использовано зелье здоровья! +{healAmount} HP");
        }
        else
        {
            Debug.LogWarning("Компонент Health не найден на игроке!");
        }
    }

    public void UseShieldPotion(float shieldAmount = 25f)
    {
        if (cachedPlayerShield != null && cachedPlayerShield.hasShieldItem)
        {
            cachedPlayerShield.currentShield = Mathf.Min(
                cachedPlayerShield.currentShield + shieldAmount,
                cachedPlayerShield.maxShield
            );
            Debug.Log($"Использовано зелье щита! +{shieldAmount} shield");
        }
        else
        {
            Debug.LogWarning("Щит не найден или не активирован!");
        }
    }

    public void RefreshPlayerComponents()
    {
        CachePlayerComponents();
    }

    // Временный метод для тестирования
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CollectArtifact();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            CollectCoins(5);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            PickupShield();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            UseHealthPotion();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            RefreshPlayerComponents();
            Debug.Log("Компоненты игрока обновлены!");
        }
    }
}