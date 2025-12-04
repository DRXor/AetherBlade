using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float moveSpeed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    public float attackWindup = 0.5f; // Добавлено: задержка перед атакой

    private Transform player;
    private Rigidbody2D rb;
    private float lastAttackTime;
    private Health playerHealth;
    private bool isAttacking = false; // Добавлено: состояние атаки
    private bool canAttack = true; // Добавлено: возможность атаковать

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
            playerHealth = player.GetComponent<Health>();
        else
            Debug.LogError("Player not found! Make sure player has 'Player' tag.");
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Преследование игрока
        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else if (distanceToPlayer <= attackRange)
        {
            // Останавливаемся для атаки
            rb.linearVelocity = Vector2.zero;

            // Атакуем если прошел кд и можем атаковать
            if (Time.time >= lastAttackTime + attackCooldown && canAttack)
            {
                StartCoroutine(AttackSequence());
                lastAttackTime = Time.time;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Заменяем AttackPlayer на корутину AttackSequence
    public IEnumerator AttackSequence()
    {
        isAttacking = true;
        canAttack = false;

        // ЗВУК: Подготовка к атаке
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySound(3, 0.6f);

        Debug.Log("Enemy preparing to attack...");

        // Ждем задержку перед атакой
        yield return new WaitForSeconds(attackWindup);

        // ЗВУК: Сама атака
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySound(4, 1f);

        // Наносим урон игроку
        if (playerHealth != null)
        {
            playerHealth.TakeDamage((int)attackDamage); // Приводим к int
            Debug.Log("Enemy attacked for " + attackDamage + " damage!");
        }

        isAttacking = false;

        // Ждем откат атаки
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Визуализация в редакторе
    void OnDrawGizmosSelected()
    {
        // Область обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Область атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}