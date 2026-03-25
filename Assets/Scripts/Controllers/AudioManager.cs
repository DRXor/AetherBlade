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
        instance = this;
    }

    void Start()
    {
        PlayMusic();
    }

    public void PlaySound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic()
    {
        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.Play();
    }
}