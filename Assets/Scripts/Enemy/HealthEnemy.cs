using UnityEngine;
using UnityEngine.Events;

public class HealthEnemy : MonoBehaviour
{
    [Header("Health Settings")]
    public float max_health = 100;
    public float current_health;
    public bool immortality = false;

    [Header("Damage Reaction Settings")]
    public bool canBeKnockbacked = true;
    public float knockbackResistance = 1f;
    public bool disableAIOnDamage = true;
    public float aiDisableDuration = 0.5f;

    [Header("Events")]
    public UnityEvent EnemyDamage;
    public UnityEvent EnemyDeath;
    public UnityEvent<Vector2> OnKnockback;

    [Header("Visual Feedback")]
    public Color damage_color = Color.white;
    public float flash_duration = 0.1f;

    private SpriteRenderer sprite_renderer;
    private Color original_color;
    private Rigidbody2D rb;
    private MonoBehaviour aiScript;

    void Start()
    {
        EnemyManager.Instance.RegisterEnemy();

        current_health = max_health;
        sprite_renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        FindAIScript();
        if (sprite_renderer != null)
            original_color = sprite_renderer.color;
    }

    void FindAIScript()
    {
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && script.GetType().Name.Contains("Enemy"))
            {
                aiScript = script;
                break;
            }
        }
    }

    public void take_damage_to_enemy(float damage)
    {
        if (immortality) return;

        Debug.Log($"=== DAMAGE CALLED ===");
        Debug.Log($"Before: {current_health}");

        current_health -= damage;

        Debug.Log($"After: {current_health}");
        Debug.Log($"Damage taken: {damage}");

        EnemyDamage?.Invoke();

        if (sprite_renderer != null)
        {
            StartCoroutine(Flash());
        }

        if (current_health <= 0)
        {
            Debug.Log($"Enemy {gameObject.name} DIED!");
            die_enemy();
        }
    }

    public void ApplyKnockback(Vector2 force, ForceMode2D forceMode = ForceMode2D.Impulse)
    {
        if (!canBeKnockbacked || rb == null) return;

        if (disableAIOnDamage && aiScript != null)
        {
            aiScript.enabled = false;
            Invoke("EnableAI", aiDisableDuration);
        }

        Vector2 adjustedForce = force / knockbackResistance;
        rb.AddForce(adjustedForce, forceMode);

        OnKnockback?.Invoke(force.normalized);

        StartCoroutine(StunEffect(0.1f));
    }

    void EnableAI()
    {
        if (aiScript != null)
            aiScript.enabled = true;
    }

    System.Collections.IEnumerator StunEffect(float duration)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color original = sprite.color;
        sprite.color = Color.yellow;

        yield return new WaitForSeconds(duration);

        if (sprite != null && current_health > 0)
            sprite.color = original;
    }

    void die_enemy()
    {
        Debug.Log($"{gameObject.name} died!");

        EnemyManager.Instance.EnemyDied();

        EnemyDeath?.Invoke();

        CreateDeathEffect();
        Destroy(gameObject, 0.1f);
    }

    void CreateDeathEffect()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = Color.gray;
        }

        if (aiScript != null)
            aiScript.enabled = false;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
    }

    System.Collections.IEnumerator Flash()
    {
        sprite_renderer.color = damage_color;
        yield return new WaitForSeconds(flash_duration);
        sprite_renderer.color = original_color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                take_damage_to_enemy(bullet.damage);

                Vector2 knockbackDirection = (transform.position - other.transform.position).normalized;
                ApplyKnockback(knockbackDirection * 3f);

                Destroy(other.gameObject);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"=== HEALTHENEMY.TakeDamage() CALLED ===");
        Debug.Log($"Object: {gameObject.name}");
        Debug.Log($"Damage: {damage}");
        Debug.Log($"Current health before: {current_health}");

        take_damage_to_enemy(damage);

        Debug.Log($"Current health after: {current_health}");
    }
}