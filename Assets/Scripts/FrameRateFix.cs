using UnityEngine;

public class FrameRateFix : MonoBehaviour
{
    [Header("Frame Rate Settings")]
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private bool useVSync = false;
    [SerializeField] private bool autoDetectMonitor = false; // Автоопределение монитора

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (autoDetectMonitor)
        {
            // Пытаемся определить частоту монитора
            int monitorRefreshRate = Screen.currentResolution.refreshRate;
            Debug.Log($"Detected monitor refresh rate: {monitorRefreshRate} Hz");

            if (monitorRefreshRate >= 120)
            {
                // Для высокочастотных мониторов используем 60 FPS
                targetFrameRate = 60;
                Debug.Log("High refresh rate monitor detected. Setting to 60 FPS for stability.");
            }
            else
            {
                // Для обычных мониторов используем их частоту
                targetFrameRate = monitorRefreshRate;
                Debug.Log($"Standard monitor. Setting to {monitorRefreshRate} FPS.");
            }
        }

        ApplyFrameRateSettings();
    }

    void ApplyFrameRateSettings()
    {
        if (!useVSync)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
            Debug.Log($"Frame rate fixed to: {targetFrameRate} FPS (VSync OFF)");
        }
        else
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1;
            Debug.Log("VSync enabled - frame rate matches monitor refresh rate");
        }

        // Дополнительные настройки для стабильности
        Application.targetFrameRate = Mathf.Clamp(targetFrameRate, 30, 144);
    }

    void Update()
    {
        // Опционально: отображаем текущий FPS для отладки
        if (Debug.isDebugBuild)
        {
            Debug.Log($"Current FPS: {1f / Time.deltaTime:F0}");
        }
    }
}