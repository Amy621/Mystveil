using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// This script handles scene transitions while ensuring save data is preserved.
/// Use this instead of directly calling SceneManager.LoadScene to ensure data is saved properly.
/// </summary>
public class SaveSceneTransition : MonoBehaviour
{
    public static SaveSceneTransition Instance { get; private set; }
    
    [Header("Settings")]
    [SerializeField] private bool saveBeforeSceneChange = true;
    [SerializeField] private bool showLoadingScreen = true;
    [SerializeField] private GameObject loadingScreenPrefab;
    
    private GameObject activeLoadingScreen;
    
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
        }
    }
    
    /// <summary>
    /// Load a scene by name while ensuring save data is preserved.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    /// <param name="saveFirst">Whether to save before changing scenes</param>
    public void LoadScene(string sceneName, bool saveFirst = true)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, saveFirst));
    }
    
    /// <summary>
    /// Load a scene by build index while ensuring save data is preserved.
    /// </summary>
    /// <param name="sceneIndex">Build index of the scene to load</param>
    /// <param name="saveFirst">Whether to save before changing scenes</param>
    public void LoadScene(int sceneIndex, bool saveFirst = true)
    {
        StartCoroutine(LoadSceneRoutine(sceneIndex, saveFirst));
    }
    
    private IEnumerator LoadSceneRoutine(string sceneName, bool saveFirst)
    {
        // Show loading screen if enabled
        if (showLoadingScreen)
        {
            ShowLoadingScreen();
        }
        
        // Save current data if enabled
        if (saveFirst && saveBeforeSceneChange && SaveManager.Instance != null)
        {
            string playerID = PlayerPrefs.GetString("ActivePlayerID", "defaultPlayer");
            SaveManager.Instance.SavePlayerData(playerID);
            Debug.Log($"Saved game data for player {playerID} before scene transition");
            
            // Small delay to ensure save completes
            yield return new WaitForSeconds(0.1f);
        }
        
        // Load the new scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // Hide loading screen once scene is loaded
        if (showLoadingScreen && activeLoadingScreen != null)
        {
            Destroy(activeLoadingScreen);
            activeLoadingScreen = null;
        }
    }
    
    private IEnumerator LoadSceneRoutine(int sceneIndex, bool saveFirst)
    {
        // Show loading screen if enabled
        if (showLoadingScreen)
        {
            ShowLoadingScreen();
        }
        
        // Save current data if enabled
        if (saveFirst && saveBeforeSceneChange && SaveManager.Instance != null)
        {
            string playerID = PlayerPrefs.GetString("ActivePlayerID", "defaultPlayer");
            SaveManager.Instance.SavePlayerData(playerID);
            Debug.Log($"Saved game data for player {playerID} before scene transition");
            
            // Small delay to ensure save completes
            yield return new WaitForSeconds(0.1f);
        }
        
        // Load the new scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // Hide loading screen once scene is loaded
        if (showLoadingScreen && activeLoadingScreen != null)
        {
            Destroy(activeLoadingScreen);
            activeLoadingScreen = null;
        }
    }
    
    private void ShowLoadingScreen()
    {
        if (loadingScreenPrefab != null)
        {
            activeLoadingScreen = Instantiate(loadingScreenPrefab);
            DontDestroyOnLoad(activeLoadingScreen);
        }
        else
        {
            Debug.LogWarning("Loading screen prefab not assigned");
        }
    }
    
    /// <summary>
    /// Static convenience method to load a scene from anywhere in your code.
    /// </summary>
    public static void LoadSceneStatic(string sceneName, bool saveFirst = true)
    {
        if (Instance != null)
        {
            Instance.LoadScene(sceneName, saveFirst);
        }
        else
        {
            Debug.LogWarning("SaveSceneTransition not found in scene. Creating one now...");
            GameObject transitionManager = new GameObject("SaveSceneTransition");
            SaveSceneTransition transition = transitionManager.AddComponent<SaveSceneTransition>();
            transition.LoadScene(sceneName, saveFirst);
        }
    }
    
    /// <summary>
    /// Static convenience method to load a scene from anywhere in your code.
    /// </summary>
    public static void LoadSceneStatic(int sceneIndex, bool saveFirst = true)
    {
        if (Instance != null)
        {
            Instance.LoadScene(sceneIndex, saveFirst);
        }
        else
        {
            Debug.LogWarning("SaveSceneTransition not found in scene. Creating one now...");
            GameObject transitionManager = new GameObject("SaveSceneTransition");
            SaveSceneTransition transition = transitionManager.AddComponent<SaveSceneTransition>();
            transition.LoadScene(sceneIndex, saveFirst);
        }
    }
} 