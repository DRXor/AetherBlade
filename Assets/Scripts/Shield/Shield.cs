using UnityEngine;
using UnityEngine.Events;

public class Shield : MonoBehaviour
{
    [Header("Shield Settings")]
    public float maxShield = 50f;
    public float currentShield;
    public bool hasShieldItem = false;
    public bool isShieldActive = false;

    [Header("Events")]
    public UnityEvent OnShieldDamage;
    public UnityEvent OnShieldBreak;
    public UnityEvent OnShieldActivate;
    public UnityEvent OnShieldDeactivate;
    public UnityEvent OnShieldPickup;

    void Start()
    {
        currentShield = 0f;
        hasShieldItem = false;
        isShieldActive = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleShield();
        }
    }

    void ToggleShield()
    {
        if (isShieldActive)
        {
            DeactivateShield();
        }
        else
        {
            ActivateShield();
        }
    }

    public bool TakeDamage(float damage)
    {
        if (!isShieldActive || currentShield <= 0)
            return false;

        currentShield -= damage;
        OnShieldDamage?.Invoke();

        if (currentShield <= 0)
        {
            currentShield = 0;
            isShieldActive = false;
            hasShieldItem = false;
            OnShieldBreak?.Invoke();
        }

        return true;
    }

    public void PickupShield(float shieldAmount)
    {
        hasShieldItem = true;
        currentShield = shieldAmount;
        OnShieldPickup?.Invoke();
    }

    public void ActivateShield()
    {
        if (hasShieldItem && !isShieldActive && currentShield > 0)
        {
            isShieldActive = true;
            OnShieldActivate?.Invoke();
        }
    }

    public void DeactivateShield()
    {
        if (isShieldActive)
        {
            isShieldActive = false;
            OnShieldDeactivate?.Invoke();
        }
    }

    public float GetShieldPercentage()
    {
        return currentShield / maxShield;
    }
}