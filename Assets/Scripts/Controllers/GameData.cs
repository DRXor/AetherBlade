using UnityEngine;

public class GameData : MonoBehaviour
{
    public GameObject playerPrefab; // Перетащи сюда префаб игрока из Project
    public int selectedSkinIndex = 0; // Пример данных

    private static GameData instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetPlayerPrefab()
    {
        return playerPrefab;
    }
}
