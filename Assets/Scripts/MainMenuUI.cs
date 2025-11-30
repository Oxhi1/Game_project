using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Main Menu UI Controller
/// Handles:
/// - New Game button
/// - Music volume slider
/// - SFX volume slider
/// - Quit button
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Text musicVolumeText;
    [SerializeField] private Text sfxVolumeText;

    [Header("Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

    private void Start()
    {
        // Setup buttons
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(StartNewGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        // Setup volume sliders
        SetupVolumeSliders();

        // Play menu music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic("menu");
        }
    }

    /// <summary>
    /// Setup volume sliders with saved values
    /// </summary>
    private void SetupVolumeSliders()
    {
        // Initialize with saved values
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.7f);

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;
            musicVolumeSlider.value = savedMusicVolume;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            UpdateMusicVolumeText(savedMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.value = savedSFXVolume;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            UpdateSFXVolumeText(savedSFXVolume);
        }
    }

    /// <summary>
    /// Called when music volume slider changes
    /// </summary>
    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
        UpdateMusicVolumeText(value);
    }

    /// <summary>
    /// Called when SFX volume slider changes
    /// </summary>
    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
            // Play a test sound when adjusting
            AudioManager.Instance.PlaySFX("button");
        }
        UpdateSFXVolumeText(value);
    }

    /// <summary>
    /// Update music volume text display
    /// </summary>
    private void UpdateMusicVolumeText(float value)
    {
        if (musicVolumeText != null)
        {
            musicVolumeText.text = "Müzik: " + Mathf.RoundToInt(value * 100) + "%";
        }
    }

    /// <summary>
    /// Update SFX volume text display
    /// </summary>
    private void UpdateSFXVolumeText(float value)
    {
        if (sfxVolumeText != null)
        {
            sfxVolumeText.text = "Ses Efektleri: " + Mathf.RoundToInt(value * 100) + "%";
        }
    }

    /// <summary>
    /// Start a new game
    /// </summary>
    public void StartNewGame()
    {
        AudioManager.Instance?.PlaySFX("button");
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
        AudioManager.Instance?.PlaySFX("button");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
