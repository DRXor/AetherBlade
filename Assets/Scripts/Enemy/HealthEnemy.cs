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
    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        current_health = max_health; // ВАЖНО: здоровье должно быть max_health
        sprite_renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        FindAIScript();

        if (sprite_renderer != null)
            original_color = sprite_renderer.color;

        Debug.Log($"{name} инициализирован. Здоровье: {current_health}/{max_health}");
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
        if (immortality || isDead)
        {
            Debug.Log($"{name}: Бессмертен или уже мертв, урон игнорируется");
            return;
        }

        Debug.Log($"=== {name} ПОЛУЧИЛ УРОН ===");
        Debug.Log($"До: {current_health}, Урон: {damage}");

        current_health -= damage;

        Debug.Log($"После: {current_health}");

        EnemyDamage?.Invoke();

        if (sprite_renderer != null)
        {
            StartCoroutine(Flash());
        }

        if (current_health <= 0)
        {
            Debug.Log($"{name} УМИРАЕТ! Текущее здоровье: {current_health}");
            die_enemy();
        }
    }

    public void ApplyKnockback(Vector2 force, ForceMode2D forceMode = ForceMode2D.Impulse)
    {
        if (!canBeKnockbacked || rb == null || isDead) return;

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
        if (aiScript != null && !isDead)
            aiScript.enabled = true;
    }

    System.Collections.IEnumerator StunEffect(float duration)
    {
        if (sprite_renderer == null) yield break;

        Color original = sprite_renderer.color;
        sprite_renderer.color = Color.yellow;

        yield return new WaitForSeconds(duration);

        if (sprite_renderer != null && current_health > 0)
            sprite_renderer.color = original;
    }

    void die_enemy()
    {
        if (isDead)
        {
            Debug.LogWarning($"{name}: Попытка умереть повторно!");
            return;
        }

        isDead = true;
        Debug.Log($"=== {name} НАЧИНАЕТ УМИРАТЬ ===");

        // 1. ОТКЛЮЧАЕМ ВСЕ СКРИПТЫ И КОМПОНЕНТЫ СРАЗУ
        Debug.Log("Отключаю все компоненты...");

        // Отключаем все MonoBehaviour кроме этого
        MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            if (script != this && script != anim) // Оставляем этот скрипт и аниматор
            {
                script.enabled = false;
                Debug.Log($"Отключил: {script.GetType().Name}");
            }
        }

        // Отключаем физику
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
            Debug.Log("Отключил Rigidbody2D");
        }

        // Отключаем коллайдеры
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
        Debug.Log("Отключил все коллайдеры");

        // 2. ЗАПУСКАЕМ АНИМАЦИЮ СМЕРТИ
        Debug.Log("Запускаю анимацию смерти...");

        if (anim == null)
        {
            anim = GetComponent<Animator>();
            Debug.Log($"Animator получен: {anim != null}");
        }

        if (anim != null)
        {
            // Проверяем параметры аниматора
            Debug.Log($"Проверяю параметры Animator (всего: {anim.parameterCount}):");
            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                Debug.Log($"  - {param.name} ({param.type})");
            }

            // Сбрасываем все параметры
            anim.SetBool("IsAttacking", false);

            // Запускаем анимацию смерти
            anim.SetBool("IsDead", true);

            // Принудительно обновляем аниматор
            anim.Update(0.1f);

            Debug.Log("Анимация смерти запущена (IsDead = true)");

            // Получаем длину анимации смерти
            float deathAnimLength = GetDeathAnimationLength();
            Debug.Log($"Длина анимации смерти: {deathAnimLength} секунд");

            // Уничтожаем после анимации
            Destroy(gameObject, deathAnimLength + 0.5f);
            Debug.Log($"Объект будет уничтожен через {deathAnimLength + 0.5f} секунд");
        }
        else
        {
            Debug.LogError("Animator не найден!");
            Destroy(gameObject, 1f);
        }

        // 3. ВЫЗЫВАЕМ СОБЫТИЯ
        EnemyDeath?.Invoke();
        Debug.Log("Событие EnemyDeath вызвано");
    }

    float GetDeathAnimationLength()
    {
        if (anim == null || anim.runtimeAnimatorController == null)
            return 1.2f; // Значение по умолчанию

        RuntimeAnimatorController ac = anim.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name.ToLower().Contains("death"))
            {
                Debug.Log($"Найден клип смерти: {clip.name}, длина: {clip.length}");
                return clip.length;
            }
        }

        Debug.LogWarning("Клип смерти не найден, использую 1.2 секунды");
        return 1.2f;
    }

    System.Collections.IEnumerator Flash()
    {
        if (sprite_renderer == null) yield break;

        sprite_renderer.color = damage_color;
        yield return new WaitForSeconds(flash_duration);

        if (sprite_renderer != null)
            sprite_renderer.color = original_color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") && !isDead)
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
        Debug.Log($"TakeDamage вызван: {damage} урона");
        take_damage_to_enemy(damage);
    }

    // Методы для отладки
    public void DebugDeath()
    {
        if (!isDead)
        {
            Debug.Log("=== ТЕСТ СМЕРТИ (вручную) ===");
            die_enemy();
        }
    }

    public void DebugTakeDamage(float damage = 25f)
    {
        if (!isDead)
        {
            Debug.Log($"=== ТЕСТ УРОНА {damage} ===");
            take_damage_to_enemy(damage);
        }
    }
}