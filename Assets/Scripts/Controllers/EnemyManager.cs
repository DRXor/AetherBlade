using UnityEngine;
using System.Collections.Generic; 

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [SerializeField] private List<GameObject> activeEnemies = new List<GameObject>();

    public int AliveEnemiesCount => activeEnemies.Count;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
            Debug.Log($"Registered: {enemy.name}. Total: {activeEnemies.Count}");
        }
    }

    public void EnemyDied(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log($"Died: {enemy.name}. Remaining: {activeEnemies.Count}");
        }

        if (activeEnemies.Count <= 0)
        {
            Debug.Log("ALL ENEMIES ARE DEAD!");
        }
    }
}