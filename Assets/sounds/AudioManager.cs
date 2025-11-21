using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sound Settings")]
    public Sounds globalSounds;
    public float masterVolume = 1f;

    void Awake()
    {
        // Синглтон паттерн
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Находим компонент Sounds если он не назначен
        if (globalSounds == null)
        {
            globalSounds = GetComponent<Sounds>();
            if (globalSounds == null)
            {
                globalSounds = gameObject.AddComponent<Sounds>();
            }
        }
    }

    // Методы для воспроизведения звуков из любого места
    public void PlaySound(int soundIndex, float volume = 1f, bool randomPitch = false)
    {
        if (globalSounds != null)
        {
            globalSounds.PlaySound(soundIndex, volume * masterVolume, randomPitch, false, 0.9f, 1.1f);
        }
    }

    public void PlaySoundAtPosition(int soundIndex, Vector3 position, float volume = 1f)
    {
        if (globalSounds != null)
        {
            // Временно перемещаем объект в нужную позицию для PlayClipAtPoint
            Vector3 originalPosition = globalSounds.transform.position;
            globalSounds.transform.position = position;
            globalSounds.PlaySound(soundIndex, volume * masterVolume, false, true);
            globalSounds.transform.position = originalPosition;
        }
    }
}
