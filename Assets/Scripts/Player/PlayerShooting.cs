using UnityEngine;


public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform weaponPivot;     
    public Transform firePoint;        
    public GameObject bulletPrefab;    
    public float fireRate = 0.2f;
    public Sounds soundsComponent;
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
            soundsComponent.PlaySound(1, 1f, false, false, 1.0f, 1.1f);
            nextFireTime = Time.time + fireRate;
            
         
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Destroy(bullet, 2f);
            
            Debug.Log("BANG!"); 
            // ������� ���������� ������
            Debug.Log("BANG!"); // ������ ����� �� �������� ������
        }
        else
        {
            Debug.Log("Missing bullet prefab or fire point!");
        }
    }
}
