using UnityEngine;
using UnityEngine.UI;

public class NoteInteraction : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject notePromptUI;     // "Нажмите Е"
    public GameObject noteTextUI;       // Текст записки
    
    [Header("Texts")]
    public string notePrompt = "Записка, нажмите Е, чтобы прочитать";
    public string noteText = "Еще одна смена. Доктор Архей сегодня проводит личный осмотр Реактора. Интересно, я хоть раз увижу его вживую? Все тут говорят о \"Протоколе Клинок\" — каком-то супероружии. Звучит как сказка.";
    
    private bool playerInRange = false;
    private bool noteActive = false;
    
    void Start()
    {
        // Отключаем UI при старте
        if (notePromptUI != null)
            notePromptUI.SetActive(false);
        
        if (noteTextUI != null)
            noteTextUI.SetActive(false);
    }
    
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!noteActive)
                ShowNoteText();
            else
                HideNoteText();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowNotePrompt();
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideNotePrompt();
            HideNoteText();
        }
    }
    
    void ShowNotePrompt()
    {
        if (notePromptUI != null)
        {
            notePromptUI.SetActive(true);
            Text text = notePromptUI.GetComponentInChildren<Text>();
            if (text != null) 
                text.text = notePrompt;
        }
    }
    
    void HideNotePrompt()
    {
        if (notePromptUI != null)
            notePromptUI.SetActive(false);
    }
    
    void ShowNoteText()
    {
        noteActive = true;
        if (noteTextUI != null)
        {
            noteTextUI.SetActive(true);
            Text text = noteTextUI.GetComponentInChildren<Text>();
            if (text != null) 
                text.text = noteText;
        }
        HideNotePrompt();
    }
    
    void HideNoteText()
    {
        noteActive = false;
        if (noteTextUI != null)
            noteTextUI.SetActive(false);
        
        if (playerInRange)
            ShowNotePrompt();
    }
}