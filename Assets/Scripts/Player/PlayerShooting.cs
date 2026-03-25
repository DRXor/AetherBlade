using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform weaponPivot;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireRate = 0.2f;

    [Header("Weapon Upgrade System")]
    public float baseDamage = 25f;
    public float damageMultiplier = 1f;
    public int bulletsPerShot = 1;
    public float spreadAngle = 0f;
    [Header("Recoil Settings")]
    public Rigidbody2D playerRb;
    public float recoilForce = 4f;
    public bool recoilPerShoot = false;
    private float nextFireTime = 0f;

    void Update()
    {
        // ������� ������ � �������
        if (weaponPivot != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - transform.position).normalized;
            weaponPivot.right = direction;
        }

        // �������� �� ���
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // �������� ������� ����� �� F
        if (Input.GetKeyDown(KeyCode.F))
        {
            PerformMeleeAttack();
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            if (recoilPerShoot) 
            {
                ApplyRecoil();
            }
            for (int i = 0; i < bulletsPerShot; i++)
            {

                // ������ ��������
                float currentSpread = (bulletsPerShot > 1) ? spreadAngle : 0f;
                float angleVariation = (i - (bulletsPerShot - 1) / 2f) * currentSpread;
                Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(0, 0, angleVariation);

                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
                AudioManager.instance.PlaySound(AudioManager.instance.shootSound);

                // ��������� ����� ����
                Bullet bulletComponent = bullet.GetComponent<Bullet>();
                if (bulletComponent != null)
                {
                    bulletComponent.damage = baseDamage * damageMultiplier;
                }
                if (!recoilPerShoot) 
                {
                    ApplyRecoil();
                }
            }

            Debug.Log("BANG!");
        }
        else
        {
            Debug.Log("Missing bullet prefab or fire point!");
        }
    }
    void ApplyRecoil()
    {
        if ((playerRb == null) && (firePoint == null))

        {
            Vector2 shootDir = firePoint.right;
            Vector2 recoilDir = -shootDir.normalized;
            playerRb.AddForce(recoilDir * recoilForce, ForceMode2D.Impulse);
        }
        else 
        {
            return;
        }
    }

    void PerformMeleeAttack()
    {
        float meleeRange = 1.5f;
        float meleeDamage = 15f;
        float meleeKnockback = 3f;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, meleeRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                HealthEnemy enemyHealth = enemy.GetComponent<HealthEnemy>();
                if (enemyHealth != null)
                {
                    enemyHealth.take_damage_to_enemy(meleeDamage);

                    // ������������ ��� ������� �����
                    Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                    enemyHealth.ApplyKnockback(knockbackDir * meleeKnockback);

                    Debug.Log($"Melee hit enemy for {meleeDamage} damage!");
                }
            }
        }
    }

    // ������ ��� �������� ������
    public void UpgradeDamage(float multiplier)
    {
        damageMultiplier *= multiplier;
    }

    public void UpgradeFireRate(float rateMultiplier)
    {
        fireRate /= rateMultiplier;
    }

    public void UpgradeSpread(float spreadReduction)
    {
        spreadAngle = Mathf.Max(0, spreadAngle - spreadReduction);
    }

    void OnDrawGizmosSelected()
    {
        // ������������ ������� ������� �����
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }

    public float GetDamageMultiplier()
    {
        // Ищем компонент Health на том же объекте
        Health playerHealth = GetComponent<Health>();
        if (playerHealth != null)
        {
            return playerHealth.damageMultiplier;
        }
        return 1f;
    }
}