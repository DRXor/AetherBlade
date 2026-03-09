using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartTextManager : MonoBehaviour
{
    public GameObject startTextPanel; // Панель с текстом
    public string startMessage = "Корпорация \"Эфир\", Сектор технического обслуживания";
    public float displayTime = 3f; // Сколько секунд показывать
    
    void Start()
    {
        StartCoroutine(ShowStartText());
    }
    
    IEnumerator ShowStartText()
    {
        // Активируем панель с текстом
        if (startTextPanel != null)
        {
            startTextPanel.SetActive(true);
            
            // Находим текстовый компонент
            Text textComponent = startTextPanel.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = startMessage;
            }
            
            // Ждём 3 секунды
            yield return new WaitForSeconds(displayTime);
            
            // Скрываем панель
            startTextPanel.SetActive(false);
        }
    }
}