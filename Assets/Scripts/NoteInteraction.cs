using UnityEngine;
using TMPro;

public class NoteInteraction : MonoBehaviour
{
    public GameObject hintText;    // Подсказка "Нажми E"
    public GameObject noteText;    // Текст записки
    private bool playerInRange = false;

    void Start()
    {
        // Гарантируем, что тексты скрыты при старте
        if (hintText) hintText.SetActive(false);
        if (noteText) noteText.SetActive(false);
    }

    void Update()
    {
        // Если игрок в зоне и нажал E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (noteText && !noteText.activeSelf)
            {
                // Показать записку
                noteText.SetActive(true);
                if (hintText) hintText.SetActive(false);
            }
            else if (noteText)
            {
                // Скрыть записку
                noteText.SetActive(false);
            }
        }

        // Закрыть записку по Escape
        if (noteText && noteText.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            noteText.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (hintText && !noteText.activeSelf)
            {
                hintText.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (hintText) hintText.SetActive(false);
            if (noteText) noteText.SetActive(false);
        }
    }
}