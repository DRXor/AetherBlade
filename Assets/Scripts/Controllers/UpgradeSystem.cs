using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem Instance;

    public PlayerStats player;

    void Awake()
    {
        Instance = this;
    }

    public void ApplyUpgrade(int id)
    {
        switch (id)
        {
            case 0:
                player.damage += 1;
                Debug.Log("Damage +1");
                break;

            case 1:
                player.speed *= 1.2f;
                Debug.Log("Speed +20%");
                break;

            case 2:
                player.maxHP += 10;
                player.currentHP += 10;
                Debug.Log("HP +10");
                break;
        }
    }
}
