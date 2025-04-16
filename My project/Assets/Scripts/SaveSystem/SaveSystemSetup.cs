using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script ensures the save system is initialized in every scene.
/// Attach this to a GameObject in your initial scene that loads first.
/// </summary>
public class SaveSystemSetup : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] public GameObject saveSystemPrefab;
    
    [Header("Auto Load Settings")]
    [SerializeField] private bool autoLoadOnStart = true;
    [SerializeField] private string defaultPlayerID = "defaultPlayer";
    
    private static bool isInitialized = false;
    
    private void Awake()
    {
        if (!isInitialized)
        {
            SetupSaveSystem();
            isInitialized = true;
            
            // Listen to scene changes to ensure save system works across scenes
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // Make this object persistent
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Save system already set up, destroy duplicate
            Destroy(gameObject);
        }
    }
    
    private void SetupSaveSystem()
    {
        // Check if save system already exists
        if (SaveManager.Instance == null)
        {
            if (saveSystemPrefab != null)
            {
                // Instantiate the save system prefab which contains all required managers
                GameObject saveSystemObj = Instantiate(saveSystemPrefab);
                saveSystemObj.name = "SaveSystem";
                DontDestroyOnLoad(saveSystemObj);
                Debug.Log("Save system instantiated from prefab");
            }
            else
            {
                // Create a new GameObject for the save system
                GameObject saveSystemObj = new GameObject("SaveSystem");
                DontDestroyOnLoad(saveSystemObj);
                
                // Add required components
                saveSystemObj.AddComponent<SaveManager>();
                saveSystemObj.AddComponent<GameManager>();
                saveSystemObj.AddComponent<QuestManager>();
                saveSystemObj.AddComponent<EnemyDropManager>();
                
                Debug.Log("Save system created with required components");
            }
        }
    }
    
    private void Start()
    {
        if (autoLoadOnStart && SaveManager.Instance != null)
        {
            // Auto load player data on start
            SaveManager.Instance.OnPlayerLogin(defaultPlayerID);
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find player character in the new scene
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        
        if (player != null)
        {
            // Ensure player has required components
            if (player.GetComponent<InventoryManager>() == null)
            {
                player.gameObject.AddComponent<InventoryManager>();
            }
            
            if (player.GetComponent<SpellManager>() == null)
            {
                player.gameObject.AddComponent<SpellManager>();
            }
            
            Debug.Log($"Required components added to player in scene: {scene.name}");
        }
    }
    
    private void OnApplicationQuit()
    {
        // Clean up when application quits
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
} 