using UnityEngine;

public class SpriteDebug : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        Debug.Log("=== SPRITE DEBUG ===");
        Debug.Log($"GameObject: {gameObject.name}");
        Debug.Log($"Position: {transform.position}");
        Debug.Log($"Has SpriteRenderer: {sr != null}");

        if (sr != null)
        {
            Debug.Log($"SpriteRenderer enabled: {sr.enabled}");
            Debug.Log($"Sprite assigned: {sr.sprite != null}");
            Debug.Log($"Sprite name: {(sr.sprite != null ? sr.sprite.name : "NULL")}");
            Debug.Log($"Sprite color: {sr.color}");
            Debug.Log($"Sorting Layer: {sr.sortingLayerName}");
            Debug.Log($"Order in Layer: {sr.sortingOrder}");
        }

        // Проверяем видимость в камере
        if (Camera.main != null)
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            Debug.Log($"Visible in camera: {viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1}");
        }
    }

    void Update()
    {
        // Рисуем луч для визуализации позиции в Scene View
        Debug.DrawRay(transform.position, Vector2.up * 2f, Color.green);
    }
}
