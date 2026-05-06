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
            DontDestroyOnLoad(gameObject); // ЭТО КЛЮЧЕВАЯ СТРОКА
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMusic();
    }

    public void PlaySound(AudioClip clip)
    {
        // Проверяем, что объект не уничтожен
        if (this == null || gameObject == null || clip == null) return;

        // Проверяем наличие AudioSource
        AudioSource source = GetComponent<AudioSource>();
        if (source == null)
        {
            Debug.LogWarning("AudioSource not found!");
            return;
        }

        // Пытаемся воспроизвести
        try
        {
            source.PlayOneShot(clip);
        }
        catch (MissingReferenceException)
        {
            Debug.LogWarning("AudioSource was destroyed, skipping sound");
        }
    }

    public void PlayMusic()
    {
        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.Play();
    }
}