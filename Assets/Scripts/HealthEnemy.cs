using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class HealthEnemy : MonoBehaviour
{
    [Header("Health Settings")]
    public float max_health = 100;
    public float current_health;
    public bool immortality = false;

    [Header("Events")]
    public UnityEvent EnemyDamage;
    public UnityEvent EnemyDeath;

    [Header("Visual Feedback")]
    public Color damage_color = Color.white;
    public float flash_duration = 0.1f;

    private SpriteRenderer sprite_renderer;
    private Color original_color;

    void Start()
    {
        current_health = max_health;
        sprite_renderer = GetComponent<SpriteRenderer>();
        if (sprite_renderer != null)
            original_color = sprite_renderer.color;

        Debug.Log($"{gameObject.name} health initialized: {current_health}/{max_health}");
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

        Debug.Log($"{gameObject} took {damage}. Health {current_health}");

        if (current_health <= 0)
        {
            die_enemy();
        }
    }

    void die_enemy() 
    {
        Debug.Log($"{gameObject.name} died!");
        EnemyDeath?.Invoke();

        Destroy(gameObject);
    }

    System.Collections.IEnumerator Flash() 
    {
        sprite_renderer.color = damage_color;
        yield return new WaitForSeconds(flash_duration);
        sprite_renderer.color = original_color;
    }

    
}
