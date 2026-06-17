using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    [Header("Results")]
    public AudioClip winClip;
    public AudioClip loseClip;

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float duckedMusicVolume = 0.25f;
    public float fadeDuration = 1f;

    private Coroutine fadeCoroutine;
    private Coroutine duckCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (musicSource != null)
                musicSource.volume = musicVolume;

            if (sfxSource != null)
                sfxSource.volume = sfxVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForCurrentScene();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForCurrentScene()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        if (sceneName == "MainMenu" || sceneName == "InfoScene")
            PlayMenuMusic();
        else
            PlayGameplayMusic();
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeMusic(clip));
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        float targetVolume = musicVolume;

        if (musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;

            while (musicSource.volume > 0f)
            {
                musicSource.volume -= startVolume * Time.unscaledDeltaTime / fadeDuration;
                yield return null;
            }
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();
        musicSource.volume = 0f;

        while (musicSource.volume < targetVolume)
        {
            musicSource.volume += targetVolume * Time.unscaledDeltaTime / fadeDuration;
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    public void PlayWin()
    {
        PlayResultSound(winClip);
    }

    public void PlayLose()
    {
        PlayResultSound(loseClip);
    }

    private void PlayResultSound(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        if (duckCoroutine != null)
            StopCoroutine(duckCoroutine);

        duckCoroutine = StartCoroutine(DuckMusicRoutine(clip));
    }

    private IEnumerator DuckMusicRoutine(AudioClip clip)
    {
        float originalVolume = musicVolume;

        if (musicSource != null)
            musicSource.volume = duckedMusicVolume;

        sfxSource.PlayOneShot(clip);

        yield return new WaitForSecondsRealtime(clip.length);

        if (musicSource != null)
        {
            float currentVolume = musicSource.volume;

            while (currentVolume < originalVolume)
            {
                currentVolume += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Clamp(currentVolume, 0f, originalVolume);
                yield return null;
            }

            musicSource.volume = originalVolume;
        }
    }

    // Compatibilidad con scripts anteriores.
    // Estos métodos se dejan vacíos para que no suenen efectos durante el juego.
    public void PlayMatch() { }
    public void PlayFail() { }
    public void PlayDrop() { }
    public void PlayFlip() { }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void ToggleMusic()
    {
        if (musicSource == null)
            return;

        if (musicSource.isPlaying)
            musicSource.Pause();
        else
            musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }

    public void ToggleMusicMute()
    {
        if (musicSource == null)
            return;

        musicSource.mute = !musicSource.mute;
    }

    public bool IsMusicMuted()
    {
        if (musicSource == null)
            return false;

        return musicSource.mute;
    }
}
