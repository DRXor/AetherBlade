using UnityEngine;

public class EnemyDebug : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"Enemy {name} setup:");
        Debug.Log($"- Tag: {tag}");
        Debug.Log($"- HealthEnemy component: {GetComponent<HealthEnemy>() != null}");
        Debug.Log($"- Collider2D: {GetComponent<Collider2D>() != null}");
        Debug.Log($"- Collider isTrigger: {GetComponent<Collider2D>()?.isTrigger}");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Enemy collision with: {collision.gameObject.name}");
    }
}