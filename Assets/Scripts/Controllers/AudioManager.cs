using UnityEngine;
using UnityEngine.SceneManagement; 

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

    public string mainMenuSceneName = "MainMenu";

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
            return; 
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuSceneName)
        {
            PlayMusic(); 
        }
        else
        {
            StopMusic(); 
        }
    }

    void Start()
    {
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        if (musicSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 1)
                musicSource = sources[1];
            else
                musicSource = gameObject.AddComponent<AudioSource>();
        }

        if (SceneManager.GetActiveScene().name == mainMenuSceneName)
        {
            PlayMusic();
        }
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

        if (musicSource.isPlaying && musicSource.clip == music) return;

        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PlayHitSound()
    {
        PlaySound(hitSound);
    }

    public void PlayEnemySound()
    {
        PlaySound(enemySound);
    }
}