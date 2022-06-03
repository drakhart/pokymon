using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioSource _musicSource;

    public static AudioManager SharedInstance;

    private void Awake() {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            print("There's more than one AudioManager instance!");

            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        _musicSource.Stop();
        _musicSource.clip = clip;
        _musicSource.Play();
    }
}
