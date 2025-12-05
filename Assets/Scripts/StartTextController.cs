using UnityEngine;
using TMPro;

public class StartDialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public float displayTime = 5f;

    void Start()
    {
        // Автоматически найдем текст, если забыли назначить
        if (dialogueText == null)
        {
            dialogueText = GetComponent<TextMeshProUGUI>();

            // Если не нашли на этом объекте, поищем в дочерних
            if (dialogueText == null)
            {
                dialogueText = GetComponentInChildren<TextMeshProUGUI>();
            }

            // Если всё равно не нашли - ошибка
            if (dialogueText == null)
            {
                Debug.LogError("Не найден TextMeshProUGUI компонент!");
                return;
            }
        }

        // ТОЛЬКО автоскрытие через 5 секунд
        Invoke("HideText", displayTime);
    }

    void HideText()
    {
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
            Debug.Log("Текст скрыт по таймеру!");
        }
    }

    // УДАЛЕН весь метод Update - никакой реакции на клавиши
}