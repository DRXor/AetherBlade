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
    public float attackWindup = 0.5f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim; // ДОБАВЛЕНО: для анимаций
    private float lastAttackTime;
    private Health playerHealth;
    private bool isAttacking = false;
    private bool canAttack = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ДОБАВЛЕНО: Получаем компонент Animator
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("Animator not found on enemy! Animations will not work.");
        }

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

            // ДОБАВЛЕНО: Передаем направление движения для анимации
            UpdateMovementAnimation(direction);
        }
        else if (distanceToPlayer <= attackRange)
        {
            // Останавливаемся для атаки
            rb.linearVelocity = Vector2.zero;

            // ДОБАВЛЕНО: Сбрасываем анимацию движения
            if (anim != null)
            {
                anim.SetFloat("MoveX", 0);
                anim.SetFloat("MoveY", 0);
            }

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

            // ДОБАВЛЕНО: Сбрасываем анимацию движения
            if (anim != null)
            {
                anim.SetFloat("MoveX", 0);
                anim.SetFloat("MoveY", 0);
            }
        }
    }

    // ДОБАВЛЕН: Метод для обновления анимации движения
    void UpdateMovementAnimation(Vector2 direction)
    {
        if (anim != null)
        {
            anim.SetFloat("MoveX", direction.x);
            anim.SetFloat("MoveY", direction.y);
        }
    }

    // Обновленная корутина атаки с анимацией
    public IEnumerator AttackSequence()
    {
        isAttacking = true;
        canAttack = false;

        // ДОБАВЛЕНО: Запускаем анимацию атаки
        if (anim != null)
        {
            anim.SetBool("IsAttacking", true);
        }

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
            playerHealth.TakeDamage((int)attackDamage);
            Debug.Log("Enemy attacked for " + attackDamage + " damage!");
        }

        // ДОБАВЛЕНО: Завершаем анимацию атаки
        if (anim != null)
        {
            anim.SetBool("IsAttacking", false);
        }

        isAttacking = false;

        // Ждем откат атаки
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // ДОБАВЛЕН: Метод для смерти врага (вызывается из HealthEnemy)
    public void OnEnemyDeath()
    {
        // Отключаем AI
        enabled = false;

        // Останавливаем движение
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Запускаем анимацию смерти
        if (anim != null)
        {
            anim.SetBool("IsDead", true);
        }

        // Отключаем коллайдеры и другие компоненты
        GetComponent<Collider2D>().enabled = false;

        // Уничтожаем объект через 2 секунды (после анимации смерти)
        Destroy(gameObject, 2f);
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