using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioSource _musicSource;
    [SerializeField] float _minRandomPitch;
    [SerializeField] float _maxRandomPitch;

    public static SoundManager SharedInstance;

    private void Awake() {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            print("There's more than one SoundManager instance!");

            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        _sfxSource.Stop();
        _sfxSource.clip = clip;
        _sfxSource.Play();
    }

    public void PlayMusic(AudioClip clip)
    {
        _musicSource.Stop();
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void PlayRandomSFX(params AudioClip[] clips)
    {
        var index = Random.Range(0, clips.Length);
        var pitch = Random.Range(_minRandomPitch, _maxRandomPitch);

        _sfxSource.pitch = pitch;

        PlaySFX(clips[index]);
    }
}
