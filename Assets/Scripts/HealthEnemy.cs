using UnityEngine;
using UnityEngine.Events;

public class HealthEnemy : MonoBehaviour
{
    [Header("Health Settings")]
    public float max_health = 100;
    public float current_health;
    public bool immortality = false;

    [Header("Knockback Settings")]
    public float knockbackResistance = 1f; // 1 = ���������� �������������, 0.5 = ������, 2 = �������
    public bool canBeKnockbacked = true;

    [Header("Events")]
    public UnityEvent EnemyDamage;
    public UnityEvent EnemyDeath;
    public UnityEvent<Vector2> OnKnockback; // ������� � ������������ ������������

    [Header("Visual Feedback")]
    public Color damage_color = Color.white;
    public float flash_duration = 0.1f;

    private SpriteRenderer sprite_renderer;
    private Color original_color;
    private Rigidbody2D rb;

    void Start()
    {
        current_health = max_health;
        sprite_renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (sprite_renderer != null)
            original_color = sprite_renderer.color;
    }

    public void take_damage_to_enemy(float damage)
    {
        if (immortality) return;

        current_health -= damage;
        EnemyDamage?.Invoke();

        if (sprite_renderer != null)
        {
            StartCoroutine(Flash());
        }

        Debug.Log($"{gameObject.name} took {damage}. Health {current_health}");

        if (current_health <= 0)
        {
            die_enemy();
        }
    }

    // ����� ��� ���������� ������������
    public void ApplyKnockback(Vector2 force, ForceMode2D forceMode = ForceMode2D.Impulse)
    {
        if (!canBeKnockbacked || rb == null) return;

        // ��������� ������������� ������������
        Vector2 adjustedForce = force / knockbackResistance;
        rb.AddForce(adjustedForce, forceMode);

        OnKnockback?.Invoke(force.normalized);

        // ��������� �� �������� ����� (�����������)
        StartCoroutine(StunEffect(0.1f));
    }

    System.Collections.IEnumerator StunEffect(float duration)
    {
        // ����� �������� ���������� ������ ���������
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color original = sprite.color;
        sprite.color = Color.yellow;

        yield return new WaitForSeconds(duration);

        if (sprite != null)
            sprite.color = original;
    }

    void die_enemy()
    {
        Debug.Log($"{gameObject.name} died!");
        EnemyDeath?.Invoke();

        // ������ ������
        CreateDeathEffect();
        Destroy(gameObject);
    }

    void CreateDeathEffect()
    {
        // ����� �������� �������, ���� � �.�.
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = Color.gray;
        }
    }

    System.Collections.IEnumerator Flash()
    {
        sprite_renderer.color = damage_color;
        yield return new WaitForSeconds(flash_duration);
        sprite_renderer.color = original_color;
    }
}