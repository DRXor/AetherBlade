using UnityEngine;
using UnityEngine.UI;

public class ShieldBarUI : MonoBehaviour
{
    [Header("Shield Bar Settings")]
    public Slider shieldSlider;
    public Shield playerShield;

    [Header("Visual Options")]
    public Color activeShieldColor = Color.cyan;
    public Color inactiveShieldColor = Color.gray;
    public Color lowShieldColor = Color.blue;

    private Image fillImage;
    private CanvasGroup canvasGroup;

    void Start()
    {
        if (shieldSlider == null)
            shieldSlider = GetComponent<Slider>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (shieldSlider != null)
        {
            fillImage = shieldSlider.fillRect.GetComponent<Image>();
            shieldSlider.minValue = 0f;
            shieldSlider.maxValue = 1f;
            shieldSlider.value = 0f;
        }

        if (playerShield != null)
        {
            SetupShieldEvents();
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerShield = player.GetComponent<Shield>();
            if (playerShield != null) SetupShieldEvents();
        }

        UpdateShieldVisibility();
    }

    void SetupShieldEvents()
    {
        playerShield.OnShieldDamage.AddListener(UpdateShieldBar);
        playerShield.OnShieldBreak.AddListener(OnShieldBreak);
        playerShield.OnShieldActivate.AddListener(OnShieldActivate);
        playerShield.OnShieldDeactivate.AddListener(OnShieldDeactivate);
        playerShield.OnShieldPickup.AddListener(OnShieldPickup);

        UpdateShieldAppearance();
    }

    void Update()
    {
        if (playerShield == null) return;

        if (shieldSlider != null)
        {
            shieldSlider.value = playerShield.GetShieldPercentage();

            if (canvasGroup != null)
            {
                bool shouldShow = playerShield.hasShieldItem;
                float targetAlpha = shouldShow ? 1f : 0f;
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * 5f);
            }
        }

        UpdateShieldAppearance();
    }

    void UpdateShieldBar()
    {
        if (shieldSlider != null && playerShield != null)
        {
            shieldSlider.value = playerShield.GetShieldPercentage();
            UpdateShieldColor();
        }
    }

    void OnShieldActivate()
    {
        Debug.Log("UI: Shield activated");
        UpdateShieldAppearance();
    }

    void OnShieldDeactivate()
    {
        Debug.Log("UI: Shield deactivated");
        UpdateShieldAppearance();
    }

    void OnShieldBreak()
    {
        Debug.Log("UI: Shield broken");
        UpdateShieldAppearance();
    }

    void OnShieldPickup()
    {
        Debug.Log("UI: Shield picked up");
        UpdateShieldAppearance();
    }

    void UpdateShieldAppearance()
    {
        if (fillImage != null && playerShield != null)
        {
            if (playerShield.isShieldActive)
            {
                float shieldPercent = playerShield.GetShieldPercentage();
                fillImage.color = Color.Lerp(lowShieldColor, activeShieldColor, shieldPercent);
            }
            else
            {
                fillImage.color = inactiveShieldColor;
            }
        }
    }

    void UpdateShieldColor()
    {
        UpdateShieldAppearance();
    }

    void UpdateShieldVisibility()
    {
        if (canvasGroup != null && playerShield != null)
        {
            canvasGroup.alpha = playerShield.hasShieldItem ? 1f : 0f;
        }
    }
}