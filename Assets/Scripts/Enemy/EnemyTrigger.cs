using UnityEngine;

public class EnemyTriggerDebug : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Enemy TRIGGER with: {other.name}, Tag: {other.tag}, Layer: {other.gameObject.layer}");
    }
}