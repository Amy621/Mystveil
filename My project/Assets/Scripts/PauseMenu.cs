using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public Slider gameAudioSlider;
    public Slider musicSlider;
    
    // Audio sources managed by this script
    private List<AudioSource> gameAudioSources = new List<AudioSource>();
    private AudioSource musicSource;
    
    // Smooth transition variables
    private float targetGameVolume;
    private float targetMusicVolume;
    private float currentGameVolume;
    private float currentMusicVolume;
    public float smoothSpeed = 5f; // Adjust this value to control smoothness (higher = faster)

    private void Awake()
    {
        // Create and set up music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = true;
        musicSource.loop = true;
        musicSource.volume = 1f;
        musicSource.spatialBlend = 0f; // 2D sound
        musicSource.priority = 1; // Lower priority than game audio
    }

    private void Start()
    {
        // Set up sliders
        if (gameAudioSlider != null)
        {
            // Set up the slider's properties
            gameAudioSlider.minValue = 0f;
            gameAudioSlider.maxValue = 100f;
            gameAudioSlider.wholeNumbers = true;
            gameAudioSlider.value = PlayerPrefs.GetFloat("GameAudioVolume", 100f);
            
            // Set up the slider's visual properties
            var colors = gameAudioSlider.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = Color.white;
            colors.selectedColor = Color.white;
            colors.disabledColor = Color.gray;
            gameAudioSlider.colors = colors;

            // Set initial volume
            currentGameVolume = gameAudioSlider.value / 100f;
            targetGameVolume = currentGameVolume;
            UpdateAllGameAudioVolumes(currentGameVolume);

            // Add listener for value changes
            gameAudioSlider.onValueChanged.RemoveAllListeners();
            gameAudioSlider.onValueChanged.AddListener(OnGameAudioVolumeChanged);
        }

        if (musicSlider != null)
        {
            // Set up the slider's properties
            musicSlider.minValue = 0f;
            musicSlider.maxValue = 100f;
            musicSlider.wholeNumbers = true;
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 100f);
            
            // Set up the slider's visual properties
            var colors = musicSlider.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = Color.white;
            colors.selectedColor = Color.white;
            colors.disabledColor = Color.gray;
            musicSlider.colors = colors;

            // Set initial volume
            currentMusicVolume = musicSlider.value / 100f;
            targetMusicVolume = currentMusicVolume;
            musicSource.volume = currentMusicVolume;

            // Add listener for value changes
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
    }

    private void Update()
    {
        // Check for backtick key press
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        // Smooth volume transitions
        if (gameAudioSources.Count > 0)
        {
            currentGameVolume = Mathf.Lerp(currentGameVolume, targetGameVolume, Time.deltaTime * smoothSpeed);
            UpdateAllGameAudioVolumes(currentGameVolume);
            Debug.Log($"Current Game Volume: {currentGameVolume}");
        }

        if (musicSource != null)
        {
            currentMusicVolume = Mathf.Lerp(currentMusicVolume, targetMusicVolume, Time.deltaTime * smoothSpeed);
            musicSource.volume = currentMusicVolume;
            Debug.Log($"Current Music Volume: {currentMusicVolume}");
        }
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnGameAudioVolumeChanged(float volume)
    {
        Debug.Log($"Game Audio Volume Changed: {volume}%");
        targetGameVolume = volume / 100f;
        currentGameVolume = targetGameVolume; // Apply immediately
        UpdateAllGameAudioVolumes(currentGameVolume); // Apply immediately
        PlayerPrefs.SetFloat("GameAudioVolume", volume);
        
        // Ensure all game audio sources are playing and not muted
        foreach (var source in gameAudioSources)
        {
            if (!source.isPlaying)
            {
                source.Play();
            }
            source.mute = false;
        }
    }

    public void OnMusicVolumeChanged(float volume)
    {
        Debug.Log($"Music Volume Changed: {volume}%");
        targetMusicVolume = volume / 100f;
        currentMusicVolume = targetMusicVolume; // Apply immediately
        musicSource.volume = currentMusicVolume; // Apply immediately
        PlayerPrefs.SetFloat("MusicVolume", volume);
        
        // Ensure the music source is playing and not muted
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
        musicSource.mute = false;
    }

    // Helper method to update all game audio volumes
    private void UpdateAllGameAudioVolumes(float volume)
    {
        foreach (var source in gameAudioSources)
        {
            source.volume = volume;
        }
    }

    // Public methods to add and manage game audio sources
    public void AddGameAudioSource(AudioSource source)
    {
        if (!gameAudioSources.Contains(source))
        {
            gameAudioSources.Add(source);
            source.spatialBlend = 1f; // 3D sound
            source.priority = 0; // High priority
            source.volume = currentGameVolume; // Set to current game volume
        }
    }

    public void RemoveGameAudioSource(AudioSource source)
    {
        gameAudioSources.Remove(source);
    }

    // Public method to set music clip
    public void SetMusicClip(AudioClip clip)
    {
        if (musicSource != null)
        {
            musicSource.clip = clip;
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }
} 