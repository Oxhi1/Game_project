using UnityEngine;

/// <summary>
/// Audio Manager singleton for managing game audio
/// Handles music and sound effects with volume control
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip battleMusic;
    [SerializeField] private AudioClip actionSFX;
    [SerializeField] private AudioClip winSFX;
    [SerializeField] private AudioClip loseSFX;
    [SerializeField] private AudioClip buttonClickSFX;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.7f;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    public float MusicVolume => musicVolume;
    public float SFXVolume => sfxVolume;

    private void Awake()
    {
        // Singleton pattern with persistence
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initialize audio settings from saved preferences
    /// </summary>
    private void Initialize()
    {
        // Load saved volume settings
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.5f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.7f);

        // Create audio sources if not assigned
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        // Apply volume settings
        ApplyVolumeSettings();
    }

    /// <summary>
    /// Apply current volume settings to audio sources
    /// </summary>
    private void ApplyVolumeSettings()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    /// <summary>
    /// Set music volume
    /// </summary>
    /// <param name="volume">Volume level (0-1)</param>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Set SFX volume
    /// </summary>
    /// <param name="volume">Volume level (0-1)</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Play background music
    /// </summary>
    /// <param name="musicName">Name of the music to play</param>
    public void PlayMusic(string musicName)
    {
        AudioClip clip = null;
        
        switch (musicName.ToLower())
        {
            case "menu":
                clip = menuMusic;
                break;
            case "battle":
                clip = battleMusic;
                break;
        }

        if (clip != null && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Stop background music
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    /// <summary>
    /// Play a sound effect
    /// </summary>
    /// <param name="sfxName">Name of the SFX to play</param>
    public void PlaySFX(string sfxName)
    {
        AudioClip clip = null;
        
        switch (sfxName.ToLower())
        {
            case "action":
                clip = actionSFX;
                break;
            case "win":
                clip = winSFX;
                break;
            case "lose":
                clip = loseSFX;
                break;
            case "button":
            case "click":
                clip = buttonClickSFX;
                break;
        }

        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    /// <summary>
    /// Play a custom audio clip as SFX
    /// </summary>
    /// <param name="clip">Audio clip to play</param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    /// <summary>
    /// Mute all audio
    /// </summary>
    public void MuteAll()
    {
        if (musicSource != null) musicSource.mute = true;
        if (sfxSource != null) sfxSource.mute = true;
    }

    /// <summary>
    /// Unmute all audio
    /// </summary>
    public void UnmuteAll()
    {
        if (musicSource != null) musicSource.mute = false;
        if (sfxSource != null) sfxSource.mute = false;
    }
}
