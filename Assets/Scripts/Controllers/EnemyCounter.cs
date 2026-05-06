using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    private int enemiesAlive = 0;

    void Start()
    {
        CountEnemies();
    }

    void CountEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesAlive = enemies.Length;
        Debug.Log($"Всего врагов на уровне: {enemiesAlive}");
    }

    public void EnemyDied()
    {
        enemiesAlive--;
        Debug.Log($"Осталось врагов: {enemiesAlive}");
    }

    public int GetRemainingEnemies()
    {
        return enemiesAlive;
    }
}