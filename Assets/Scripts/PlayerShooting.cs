using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform weaponPivot;      // ���� �������� WeaponPivot
    public Transform firePoint;        // ���� �������� FirePoint  
    public GameObject bulletPrefab;    // ���� �������� ������ ���� (�������� �����)
    public float fireRate = 0.2f;      // ���������������� (��������� � �������)

    private float nextFireTime = 0f;
    

    void Update()
    {
        // ������� ������ � �������
        if (weaponPivot != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - transform.position).normalized;
            weaponPivot.right = direction; // ������������ ������ � ������� ����
        }

        // �������� �� ���
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Destroy(bullet, 2f);

            // ������� ���������� ������
            Debug.Log("BANG!"); // ������ ����� �� �������� ������
        }
        else
        {
            Debug.Log("Missing bullet prefab or fire point!");
        }
    }
}
