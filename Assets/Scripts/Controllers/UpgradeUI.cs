using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    public GameObject panel;

    void Start()
    {
        panel.SetActive(false);
    }

    public void Show()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void SelectUpgrade(int id)
    {
        UpgradeSystem.Instance.ApplyUpgrade(id);

        panel.SetActive(false);
        Time.timeScale = 1f;

        // оепеунд мю якедсчыхи спнбемэ
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextLevel();
        }
    }
}