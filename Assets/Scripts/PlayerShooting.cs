using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform weaponPivot;      // Ñþäà ïåðåòàùè WeaponPivot
    public Transform firePoint;        // Ñþäà ïåðåòàùè FirePoint  
    public GameObject bulletPrefab;    // Ñþäà ïåðåòàùè ïðåôàá ïóëè (ñîçäàäèì ïîçæå)
    public float fireRate = 0.2f;      // Ñêîðîñòðåëüíîñòü (âûñòðåëîâ â ñåêóíäó)

    private float nextFireTime = 0f;


    void Update()
    {
        // Ïîâîðîò îðóæèÿ ê êóðñîðó
        if (weaponPivot != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - transform.position).normalized;
            weaponPivot.right = direction; // Ïîâîðà÷èâàåì îðóæèå â ñòîðîíó ìûøè
        }

        // Ñòðåëüáà ïî ËÊÌ
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

            // Ïðîñòîé âèçóàëüíûé ýôôåêò
            Debug.Log("BANG!"); // Çàìåíè ïîòîì íà ðåàëüíûé ýôôåêò
        }
        else
        {
            Debug.Log("Missing bullet prefab or fire point!");
        }
    }
}
