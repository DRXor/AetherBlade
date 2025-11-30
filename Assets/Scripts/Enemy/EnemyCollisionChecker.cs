using UnityEngine;

public class EnemyCollisionChecker : MonoBehaviour
{
    void Update()
    {
        // Показываем границы коллайдера
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Bounds bounds = collider.bounds;
            Debug.DrawLine(new Vector2(bounds.min.x, bounds.min.y), new Vector2(bounds.max.x, bounds.min.y), Color.green);
            Debug.DrawLine(new Vector2(bounds.max.x, bounds.min.y), new Vector2(bounds.max.x, bounds.max.y), Color.green);
            Debug.DrawLine(new Vector2(bounds.max.x, bounds.max.y), new Vector2(bounds.min.x, bounds.max.y), Color.green);
            Debug.DrawLine(new Vector2(bounds.min.x, bounds.max.y), new Vector2(bounds.min.x, bounds.min.y), Color.green);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Enemy trigger with: {other.gameObject.name} (Tag: {other.tag})");
    }
}
