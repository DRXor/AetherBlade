using UnityEngine;
using UnityEngine.Events;

public class Shield : MonoBehaviour
{
    [Header("Shield Settings")]
    public float maxShield = 50f;
    public float currentShield;
    public bool hasShield = false;

    [Header("Events")]
    public UnityEvent OnShieldDamage;
    public UnityEvent OnShieldBreak;
    public UnityEvent OnShieldRestore;

    void Start()
    {
        currentShield = 0f;
        hasShield = false;
    }

    public bool TakeDamage(float damage)
    {
        if (!hasShield || currentShield <= 0)
            return false;

        currentShield -= damage;
        OnShieldDamage?.Invoke();

        Debug.Log($"{gameObject.name} took {damage} shield damage. Shield: {currentShield}");

        if (currentShield <= 0)
        {
            currentShield = 0;
            hasShield = false; 
            OnShieldBreak?.Invoke();
            Debug.Log($"{gameObject.name}'s shield broke permanently!");
        }

        return true;
    }

    public void AddShield(float shieldAmount)
    {
        hasShield = true;
        currentShield = Mathf.Min(currentShield + shieldAmount, maxShield);
        OnShieldRestore?.Invoke();

        Debug.Log($"{gameObject.name} gained shield: {currentShield}/{maxShield}");
    }

    public void RemoveShield()
    {
        hasShield = false;
        currentShield = 0f;
        Debug.Log($"{gameObject.name}'s shield removed");
    }

    public float GetShieldPercentage()
    {
        return currentShield / maxShield;
    }
}