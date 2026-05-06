using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource sfxSource;
    public AudioSource musicSource;

    public AudioClip shootSound;
    public AudioClip hitSound;
    public AudioClip pickupSound;
    public AudioClip enemySound;
    public AudioClip music;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Автоматически находим или создаём AudioSource-ы
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        if (musicSource == null)
        {
            // Ищем отдельный источник для музыки
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 1)
                musicSource = sources[1];
            else
                musicSource = gameObject.AddComponent<AudioSource>();
        }

        PlayMusic();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        try
        {
            sfxSource.PlayOneShot(clip);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to play sound: {e.Message}");
        }
    }

    public void PlayMusic()
    {
        if (musicSource == null || music == null) return;

        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Добавьте этот метод для проверки
    public void PlayHitSound()
    {
        PlaySound(hitSound);
    }

    public void PlayEnemySound()
    {
        PlaySound(enemySound);
    }
}