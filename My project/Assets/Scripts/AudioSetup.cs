using UnityEngine;

public class AudioSetup : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip[] gameAudioClips;  // Array of game audio clips
    public AudioClip musicClip;          // Background music clip

    private PauseMenu pauseMenu;
    private AudioSource[] gameAudioSources;

    void Start()
    {
        // Get the PauseMenu component
        pauseMenu = GetComponent<PauseMenu>();
        
        if (pauseMenu != null)
        {
            // Set up music
            if (musicClip != null)
            {
                pauseMenu.SetMusicClip(musicClip);
            }

            // Set up game audio sources
            if (gameAudioClips != null && gameAudioClips.Length > 0)
            {
                gameAudioSources = new AudioSource[gameAudioClips.Length];
                
                // Create an audio source for each game audio clip
                for (int i = 0; i < gameAudioClips.Length; i++)
                {
                    if (gameAudioClips[i] != null)
                    {
                        // Create a new GameObject for each audio source
                        GameObject audioObj = new GameObject($"GameAudioSource_{i}");
                        audioObj.transform.SetParent(transform);
                        
                        // Add and set up the AudioSource
                        AudioSource source = audioObj.AddComponent<AudioSource>();
                        source.clip = gameAudioClips[i];
                        source.playOnAwake = true;
                        source.loop = true;
                        
                        // Add the audio source to the pause menu system
                        pauseMenu.AddGameAudioSource(source);
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        // Clean up game audio sources
        if (gameAudioSources != null)
        {
            foreach (var source in gameAudioSources)
            {
                if (source != null && pauseMenu != null)
                {
                    pauseMenu.RemoveGameAudioSource(source);
                }
            }
        }
    }
} 