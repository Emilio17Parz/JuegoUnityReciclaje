using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("SFX")]
    public AudioClip flipClip;
    public AudioClip matchClip;
    public AudioClip failClip;
    public AudioClip winClip;
    public AudioClip dropClip;
    [Header("Music")]
    public AudioClip backgroundMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (backgroundMusic != null)
            PlayMusic(backgroundMusic);
    }

    public void PlayFlip() => PlaySFX(flipClip);
    public void PlayMatch() => PlaySFX(matchClip);
    public void PlayFail() => PlaySFX(failClip);
    public void PlayWin() => PlaySFX(winClip);

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void ToggleMusic()
    {
        if (musicSource == null) return;

        if (musicSource.isPlaying)
            musicSource.Pause();
        else
            musicSource.Play();
    }
    public void PlayDrop()
    {
        PlaySFX(dropClip);
    }
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = volume;
    }
}