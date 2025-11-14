using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using System.Security.Cryptography.X509Certificates;

public class Sounds : MonoBehaviour
{
    public AudioClip[] sounds;
    public SoundArray[] randSound;
    private AudioSource audioSrc => GetComponent<AudioSource>();
    public void PlaySound(int i, float volume = 1f, bool random = false, bool destroyed = false, float p1 = 0.85f, float p2 = 1.2f)
    {
        AudioClip clip = random ? randSound[i].soundArray[Random.Range(0, randSound[i].soundArray.Length)] : sounds[i];
        audioSrc.pitch = Random.Range(p1, p2);
        if (destroyed)
            AudioSource.PlayClipAtPoint(clip, transform.position, volume);
        else
            audioSrc.PlayOneShot(clip, volume);
    }

    [System.Serializable]
    public class SoundArray
    {
        public AudioClip[] soundArray;
    }
}
