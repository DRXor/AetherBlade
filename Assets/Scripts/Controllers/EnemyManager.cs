using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public int aliveEnemies = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterEnemy()
    {
        aliveEnemies++;
        Debug.Log("Enemy Registered! Total: " + aliveEnemies);
    }

    public void EnemyDied()
    {
        aliveEnemies--;
        Debug.Log("Enemy Died! Remaining: " + aliveEnemies);

        if (aliveEnemies <= 0)
        {
            Debug.Log("ALL ENEMIES ARE DEAD!");
        }
    }
}