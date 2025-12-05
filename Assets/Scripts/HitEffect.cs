using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    public float lifetime = 0.3f;
    public float startScale = 0.5f;
    public float endScale = 1.5f;

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.one * startScale;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / lifetime;

        // Анимация масштаба
        transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, progress);

        // Анимация прозрачности
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(1f, 0f, progress);
            spriteRenderer.color = color;
        }
    }
}