using UnityEngine;
using TMPro;
using System.Collections;

public class StoryText : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public string[] storyLines; // Массив строк
    public float timePerLine = 3f;

    void Start()
    {
        StartCoroutine(ShowStory());
    }

    IEnumerator ShowStory()
    {
        foreach (string line in storyLines)
        {
            storyText.text = line;
            yield return new WaitForSeconds(timePerLine);
        }

        // Скрыть после всех строк
        storyText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Пропуск по любой клавише
        if (Input.anyKeyDown)
        {
            StopAllCoroutines();
            storyText.gameObject.SetActive(false);
        }
    }
}
