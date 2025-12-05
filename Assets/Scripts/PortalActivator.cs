using UnityEngine;

public class PortalActivator : MonoBehaviour
{
    public GameObject portal;

    void Start()
    {
        if (portal != null)
            portal.SetActive(false);
    }

    void Update()
    {
        if (EnemyManager.Instance != null &&
            EnemyManager.Instance.aliveEnemies == 0)
        {
            if (portal != null && !portal.activeSelf)
                portal.SetActive(true);
        }
    }
}