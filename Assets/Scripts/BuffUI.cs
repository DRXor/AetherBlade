using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BuffUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text timerText;

    private Coroutine timerCoroutine;

    void Start()
    {
        icon.enabled = true;
        timerText.enabled = true;

        icon.color = Color.red; // чтобы точно увидеть
        timerText.text = "TEST";
    }

    public void ShowBuff(Sprite sprite, float duration)
    {
        icon.sprite = sprite;
        icon.enabled = true;
        timerText.enabled = true;

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(Timer(duration));
    }

    IEnumerator Timer(float duration)
    {
        float time = duration;

        while (time > 0)
        {
            timerText.text = Mathf.Ceil(time) + "s";
            time -= Time.deltaTime;
            yield return null;
        }

        icon.enabled = false;
        timerText.enabled = false;
    }
}