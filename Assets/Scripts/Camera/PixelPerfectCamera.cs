using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PixelPerfectCamera : MonoBehaviour
{
    [Header("Pixel Perfect Settings")]
    public int pixelsPerUnit = 16;
    public int referenceHeight = 180;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        SetupPixelPerfect();
    }

    void SetupPixelPerfect()
    {
        cam.orthographic = true;
        cam.orthographicSize = referenceHeight / (2f * pixelsPerUnit);

        // Настройка для четких пикселей
        cam.nearClipPlane = 0;
        cam.farClipPlane = 100;
    }
}
