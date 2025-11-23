using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("Inventory Stats")]
    public int artifactsCollected = 0;
    public int coinsCollected = 0;
    public int keysCollected = 0;

    [Header("Active Items")]
    public bool hasShield = false;

    private Shield cachedPlayerShield;
    private Health cachedPlayerHealth;

    public event Action<int> OnArtifactsChanged;
    public event Action<int> OnCoinsChanged;
    public event Action<int> OnKeysChanged;
    public event Action<bool> OnShieldChanged;

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
    }

    public void CollectCoins(int amount = 1)
    {
        coinsCollected += amount;
        OnCoinsChanged?.Invoke(coinsCollected);
    }

    public void CollectKey()
    {
        keysCollected++;
        OnKeysChanged?.Invoke(keysCollected);
    }

    public void PickupShield()
    {
        if (!hasShield)
        {
            hasShield = true;
            OnShieldChanged?.Invoke(true);

            if (cachedPlayerShield != null)
            {
                cachedPlayerShield.PickupShield(50f);
            }
        }
    }

    public void UseHealthPotion(int healAmount = 25)
    {
        if (cachedPlayerHealth != null)
        {
            cachedPlayerHealth.Heal(healAmount);
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
        }
    }

    public void RefreshPlayerComponents()
    {
        CachePlayerComponents();
    }
}