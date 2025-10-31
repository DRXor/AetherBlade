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

    public bool TakeDamage(float damage)
    {
        if (!isShieldActive || currentShield <= 0)
            return false;

        currentShield -= damage;
        OnShieldDamage?.Invoke();

        Debug.Log($"{gameObject.name} took {damage} shield damage. Shield: {currentShield}");

        if (currentShield <= 0)
        {
            currentShield = 0;
            isShieldActive = false;
            hasShieldItem = false; 
            OnShieldBreak?.Invoke();
            Debug.Log($"{gameObject.name}'s shield broke permanently!");
        }

        return true;
    }

    public void PickupShield(float shieldAmount)
    {
        hasShieldItem = true;
        currentShield = shieldAmount;
        OnShieldPickup?.Invoke();

        Debug.Log($"��� ��������! ���������: {currentShield}/{maxShield}");
    }

    public void ActivateShield()
    {
        if (hasShieldItem && !isShieldActive && currentShield > 0)
        {
            isShieldActive = true;
            OnShieldActivate?.Invoke();
            Debug.Log($"��� �����������! ���������: {currentShield}/{maxShield}");
        }
        else if (!hasShieldItem)
        {
            Debug.Log("��� ���� � ���������!");
        }
        else if (isShieldActive)
        {
            Debug.Log("��� ��� �����������!");
        }
    }

    public void DeactivateShield()
    {
        if (isShieldActive)
        {
            isShieldActive = false;
            OnShieldDeactivate?.Invoke();
            Debug.Log("��� �������������");
        }
    }

    public float GetShieldPercentage()
    {
        return currentShield / maxShield;
    }

    public void LogStatus()
    {
        Debug.Log($"������: � ���������={hasShieldItem}, �������={isShieldActive}, ���������={currentShield}/{maxShield}");
    }
}