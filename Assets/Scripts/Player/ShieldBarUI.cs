using UnityEngine;
using UnityEngine.UI;

public class ShieldBarUI : MonoBehaviour
{
    [Header("Shield Bar Settings")]
    public Slider shieldSlider;
    public Shield playerShield;

    [Header("Visual Options")]
    public Color shieldColor = Color.cyan;
    public Color lowShieldColor = Color.blue;

    private Image fillImage;
    private CanvasGroup canvasGroup;

    void Start()
    {
        Debug.Log("ShieldBarUI Start called");

        if (shieldSlider == null)
        {
            shieldSlider = GetComponent<Slider>();
            Debug.Log("Slider found: " + (shieldSlider != null));
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (shieldSlider != null)
        {
            fillImage = shieldSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = shieldColor;
            }

            shieldSlider.minValue = 0f;
            shieldSlider.maxValue = 1f;
            shieldSlider.value = 0f;
        }

        if (playerShield != null)
        {
            SetupShieldEvents();
            Debug.Log("Using playerShield from inspector");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerShield = player.GetComponent<Shield>();
            if (playerShield != null)
            {
                SetupShieldEvents();
                Debug.Log("Found player by tag and shield component");
                return;
            }
            else
            {
                Debug.LogWarning("Player found but Shield component missing!");
            }
        }
        else
        {
            Debug.LogWarning("No GameObject with tag 'Player' found!");
        }

        playerShield = FindFirstObjectByType<Shield>();
        if (playerShield != null)
        {
            SetupShieldEvents();
            Debug.Log("Found shield component in scene");
            return;
        }

        Debug.LogError("Could not find Shield component! Please assign manually.");
    }

    void SetupShieldEvents()
    {
        if (playerShield != null)
        {
            playerShield.OnShieldDamage.AddListener(UpdateShieldBar);
            playerShield.OnShieldBreak.AddListener(OnShieldBreak);
            playerShield.OnShieldRestore.AddListener(OnShieldRestore);
            UpdateShieldVisibility();
            Debug.Log("Shield events setup successfully");
        }
    }

    void Update()
    {
        if (playerShield == null) return;

        if (shieldSlider != null)
        {
            shieldSlider.value = playerShield.GetShieldPercentage();

            if (canvasGroup != null)
            {
                float targetAlpha = playerShield.hasShield ? 1f : 0f;
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * 5f);
            }
        }
    }

    void UpdateShieldBar()
    {
        if (shieldSlider != null && playerShield != null)
        {
            shieldSlider.value = playerShield.GetShieldPercentage();
            UpdateShieldColor();
        }
    }

    void OnShieldBreak()
    {
        Debug.Log("Shield broken!");
        UpdateShieldVisibility();
    }

    void OnShieldRestore()
    {
        Debug.Log("Shield restored!");
        UpdateShieldVisibility();
    }

    void UpdateShieldColor()
    {
        if (fillImage != null && playerShield != null)
        {
            float shieldPercent = playerShield.GetShieldPercentage();
            fillImage.color = Color.Lerp(lowShieldColor, shieldColor, shieldPercent);
        }
    }

    void UpdateShieldVisibility()
    {
        if (canvasGroup != null && playerShield != null)
        {
            canvasGroup.alpha = playerShield.hasShield ? 1f : 0f;
        }
    }

    void OnDestroy()
    {
        if (playerShield != null)
        {
            playerShield.OnShieldDamage.RemoveListener(UpdateShieldBar);
            playerShield.OnShieldBreak.RemoveListener(OnShieldBreak);
            playerShield.OnShieldRestore.RemoveListener(OnShieldRestore);
        }
    }
}