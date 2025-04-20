using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Collections.Generic;

public class SimpleSaveSystem : MonoBehaviour
{
    public static SimpleSaveSystem Instance { get; private set; }

    // Declare delegates and events for save/load operations
    public delegate void SaveLoadDelegate(SimpleSaveData saveData);
    public event SaveLoadDelegate OnSave;
    public event SaveLoadDelegate OnLoad;

    [Header("Save Settings")]
    [SerializeField] private string saveFileName = "save.json";
    [SerializeField] private KeyCode saveLoadKey = KeyCode.Escape;
    
    // Private backing field for the save menu UI
    [SerializeField] private GameObject _saveMenuUI;
    
    // Public property with getter and setter
    public GameObject SaveMenuUI 
    {
        get { return _saveMenuUI; }
        set { _saveMenuUI = value; }
    }
    
    private string savePath;
    private bool isSaveMenuOpen = false;
    
    // Player references
    private GameObject player;
    private MonoBehaviour playerBehavior; // Reference to whatever script handles player health/stats

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSaveSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeSaveSystem()
    {
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find player in the new scene
        FindPlayerReferences();
    }
    
    private void FindPlayerReferences()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Try to find any player script that handles health/mana
            // This approach is more flexible than looking for a specific class
            playerBehavior = player.GetComponent<MonoBehaviour>();
            Debug.Log("Player found: " + player.name);
        }
        else
        {
            Debug.Log("Player not found in scene. This is normal for menu scenes.");
        }
    }

    private void Update()
    {
        // Toggle save menu with Escape key
        if (Input.GetKeyDown(saveLoadKey))
        {
            ToggleSaveMenu();
        }
    }
    
    public void ToggleSaveMenu()
    {
        if (_saveMenuUI != null)
        {
            isSaveMenuOpen = !isSaveMenuOpen;
            _saveMenuUI.SetActive(isSaveMenuOpen);
            
            // Pause game when menu is open
            Time.timeScale = isSaveMenuOpen ? 0 : 1;
        }
        else
        {
            Debug.LogWarning("Save menu UI reference is missing!");
        }
    }
    
    public void SaveGame()
    {
        try
        {
            if (player == null)
            {
                FindPlayerReferences();
                if (player == null)
                {
                    Debug.LogWarning("Cannot save: No player found in scene");
                    return;
                }
            }
            
            SimpleSaveData saveData = new SimpleSaveData();
            
            // Save current scene
            saveData.currentScene = SceneManager.GetActiveScene().name;
            
            // Save player position
            saveData.positionX = player.transform.position.x;
            saveData.positionY = player.transform.position.y;
            saveData.positionZ = player.transform.position.z;
            
            // Invoke the OnSave event for all listeners
            OnSave?.Invoke(saveData);
            
            // Find components for saving stats and spells
            SavePlayerStats(saveData);
            SavePlayerSpells(saveData);
            
            // Add timestamp
            saveData.saveTime = DateTime.Now;
            
            // Convert to JSON
            string json = JsonUtility.ToJson(saveData, true);
            
            // Write to file
            File.WriteAllText(savePath, json);
            
            Debug.Log("Game saved successfully to: " + savePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving game: " + e.Message);
        }
    }
    
    private void SavePlayerStats(SimpleSaveData saveData)
    {
        // Try to get player name
        PlayerStats playerScriptableObject = null;
        var playerCharacter = player.GetComponent<PlayerCharacter>();
        if (playerCharacter != null)
        {
            saveData.playerName = playerCharacter.GetPlayerName();
            playerScriptableObject = playerCharacter.GetPlayerStats();
        }
        
        // Try to get health/mana/stats from components
        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            saveData.health = playerHealth.CurrentHealth;
            saveData.maxHealth = playerHealth.MaxHealth;
        }
        else if (playerScriptableObject != null)
        {
            // Fall back to scriptable object values
            saveData.maxHealth = playerScriptableObject.HP;
            saveData.health = saveData.maxHealth; // Assume full health if no component
        }
        
        var playerMana = player.GetComponent<PlayerMana>();
        if (playerMana != null)
        {
            saveData.mana = playerMana.CurrentMana;
            saveData.maxMana = playerMana.MaxMana;
        }
        else if (playerScriptableObject != null)
        {
            // Fall back to scriptable object values
            saveData.maxMana = playerScriptableObject.MANA;
            saveData.mana = saveData.maxMana; // Assume full mana if no component
        }
        
        var playerLevel = player.GetComponent<PlayerLevel>();
        if (playerLevel != null)
        {
            saveData.level = playerLevel.CurrentLevel;
        }
        else if (playerScriptableObject != null)
        {
            saveData.level = 1; // Default level if no component
        }
        
        // Save combat stats
        var playerCombatStats = player.GetComponent<PlayerCombatStatsAdapter>();
        if (playerCombatStats != null)
        {
            saveData.attackPoints = playerCombatStats.AttackPoints;
            saveData.defensePoints = playerCombatStats.DefensePoints;
            saveData.specialAttackPoints = playerCombatStats.SpecialAttackPoints;
            saveData.specialDefensePoints = playerCombatStats.SpecialDefensePoints;
            saveData.speed = playerCombatStats.Speed;
        }
        else if (playerScriptableObject != null)
        {
            // Fall back to scriptable object values
            saveData.attackPoints = playerScriptableObject.ATK;
            saveData.defensePoints = playerScriptableObject.DEF;
            saveData.specialAttackPoints = playerScriptableObject.SPA;
            saveData.specialDefensePoints = playerScriptableObject.SPD;
            saveData.speed = playerScriptableObject.SPE;
        }
        
        // If you have a charisma stat, save it
        var playerAttributes = player.GetComponent<PlayerAttributesAdapter>();
        if (playerAttributes != null)
        {
            saveData.charisma = playerAttributes.Charisma;
        }
        else if (playerScriptableObject != null)
        {
            // Fall back to scriptable object values
            saveData.charisma = playerScriptableObject.CHA;
        }
    }
    
    private void SavePlayerSpells(SimpleSaveData saveData)
    {
        // Try to get spell information from SpellManager
        var spellManager = player.GetComponent<SpellManager>();
        if (spellManager != null)
        {
            saveData.unlockedSpells = spellManager.GetUnlockedSpells();
            saveData.equippedSpells = spellManager.GetEquippedSpells();
        }
    }
    
    public void LoadGame()
    {
        try
        {
            if (!File.Exists(savePath))
            {
                Debug.LogWarning("No save file found at: " + savePath);
                return;
            }
            
            // Read JSON from file
            string json = File.ReadAllText(savePath);
            
            // Convert JSON to save data
            SimpleSaveData saveData = JsonUtility.FromJson<SimpleSaveData>(json);
            
            // Load the correct scene if we're not already in it
            if (SceneManager.GetActiveScene().name != saveData.currentScene)
            {
                // Store the save data temporarily while loading scene
                StartCoroutine(LoadSceneAndPosition(saveData));
            }
            else
            {
                // Apply data directly if we're already in the correct scene
                ApplySaveData(saveData);
            }
            
            Debug.Log("Game loaded successfully from: " + savePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading game: " + e.Message);
        }
    }
    
    private System.Collections.IEnumerator LoadSceneAndPosition(SimpleSaveData saveData)
    {
        // Load the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(saveData.currentScene);
        
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // Wait one more frame for all objects to initialize
        yield return new WaitForEndOfFrame();
        
        // Find player in the new scene
        FindPlayerReferences();
        
        // Apply the save data
        ApplySaveData(saveData);
    }
    
    private void ApplySaveData(SimpleSaveData saveData)
    {
        if (player == null)
        {
            FindPlayerReferences();
            if (player == null)
            {
                Debug.LogError("Cannot apply save data: No player found in scene");
                return;
            }
        }
        
        // Set player position
        Vector3 position = new Vector3(saveData.positionX, saveData.positionY, saveData.positionZ);
        player.transform.position = position;
        
        // Invoke the OnLoad event for all listeners
        OnLoad?.Invoke(saveData);
        
        // Apply stats and spells
        ApplyPlayerStats(saveData);
        ApplyPlayerSpells(saveData);
        
        // Close the save menu if it's open
        if (isSaveMenuOpen)
        {
            ToggleSaveMenu();
        }
    }
    
    private void ApplyPlayerStats(SimpleSaveData saveData)
    {
        // Apply health/mana/stats to various possible components
        
        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.MaxHealth = saveData.maxHealth;
            playerHealth.CurrentHealth = saveData.health;
        }
        
        var playerMana = player.GetComponent<PlayerMana>();
        if (playerMana != null)
        {
            playerMana.MaxMana = saveData.maxMana;
            playerMana.CurrentMana = saveData.mana;
        }
        
        var playerLevel = player.GetComponent<PlayerLevel>();
        if (playerLevel != null)
        {
            playerLevel.CurrentLevel = saveData.level;
        }
        
        // Apply combat stats
        var playerCombatStats = player.GetComponent<PlayerCombatStatsAdapter>();
        if (playerCombatStats != null)
        {
            playerCombatStats.AttackPoints = saveData.attackPoints;
            playerCombatStats.DefensePoints = saveData.defensePoints;
            playerCombatStats.SpecialAttackPoints = saveData.specialAttackPoints;
            playerCombatStats.SpecialDefensePoints = saveData.specialDefensePoints;
            playerCombatStats.Speed = saveData.speed;
        }
        
        // If you have a charisma stat, load it
        var playerAttributes = player.GetComponent<PlayerAttributesAdapter>();
        if (playerAttributes != null)
        {
            playerAttributes.Charisma = saveData.charisma;
        }
        
        // Set player name if applicable
        var playerCharacter = player.GetComponent<PlayerCharacter>();
        if (playerCharacter != null && !string.IsNullOrEmpty(saveData.playerName))
        {
            playerCharacter.SetPlayerName(saveData.playerName);
        }
    }
    
    private void ApplyPlayerSpells(SimpleSaveData saveData)
    {
        // Apply spell information to SpellManager
        var spellManager = player.GetComponent<SpellManager>();
        if (spellManager != null)
        {
            spellManager.LoadUnlockedSpells(saveData.unlockedSpells);
            spellManager.LoadEquippedSpells(saveData.equippedSpells);
        }
    }

    // Method to get the last loaded scene
    public string GetLastLoadedScene()
    {
        try
        {
            if (!File.Exists(savePath))
            {
                return "GameScene"; // Default scene if no save exists
            }
            
            // Read the save file to get the scene name
            string json = File.ReadAllText(savePath);
            SimpleSaveData saveData = JsonUtility.FromJson<SimpleSaveData>(json);
            
            if (string.IsNullOrEmpty(saveData.currentScene))
            {
                return "GameScene"; // Default scene if scene name is empty
            }
            
            return saveData.currentScene;
        }
        catch (Exception)
        {
            return "GameScene"; // Default scene if there's an error
        }
    }
} 