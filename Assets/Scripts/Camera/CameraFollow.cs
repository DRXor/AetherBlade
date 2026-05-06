using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

    void Start()
    {
        // Ищем игрока при старте
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("CameraFollow: Player found by tag!");
            }
            else
            {
                Debug.LogWarning("CameraFollow: No player with tag 'Player' found!");
            }
        }
    }

    void LateUpdate()
    {
        if (player == null)
        {
            // Продолжаем искать каждый кадр, пока не найдём
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("CameraFollow: Player found in LateUpdate!");
            }
            else
            {
                return;
            }
        }

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}