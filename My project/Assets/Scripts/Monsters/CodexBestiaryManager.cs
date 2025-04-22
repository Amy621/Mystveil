using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the player's bestiary, tracking which monsters have been discovered
/// and their discovery status. Saves/loads data via the SimpleSaveSystem.
/// </summary>
public class CodexBestiaryManager : MonoBehaviour
{
    [Tooltip("Reference to all monster scriptable objects in the game")]
    [SerializeField] private List<ScriptableMonster> allMonsters = new List<ScriptableMonster>();
    
    [Tooltip("Automatically populate the monster list in the editor")]
    [SerializeField] private bool autoPopulateInEditor = true;
    
    // Dictionary to track monster discovery status - stores monster ID and discovery state
    private static Dictionary<string, MonsterDiscoveryStatus> discoveredMonsters = new Dictionary<string, MonsterDiscoveryStatus>();
    
    // Singleton instance
    private static CodexBestiaryManager instance;
    
    // Track whether we've loaded data yet
    private static bool hasLoadedData = false;
    
    // Delegate for monster discovery events
    public delegate void MonsterDiscoveryEvent(string monsterId, MonsterDiscoveryStatus status);
    
    // Event fired when a monster is discovered or its status changes
    public static event MonsterDiscoveryEvent OnMonsterDiscoveryChanged;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Auto-populate monster list in editor
        // if (autoPopulateInEditor && allMonsters.Count == 0 && Application.isEditor)
        // {
        //     PopulateMonsterList();
        // }
        
        // Load saved bestiary data
        LoadBestiaryData();
    }
    
    #if UNITY_EDITOR
    private void PopulateMonsterList()
    {
        // Find all monster scriptable objects in the project
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:ScriptableMonster");
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            ScriptableMonster monster = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableMonster>(path);
            if (monster != null && !allMonsters.Contains(monster))
            {
                allMonsters.Add(monster);
            }
        }
    }
    #endif
    
    /// <summary>
    /// Load bestiary data from the save system
    /// </summary>
    public void LoadBestiaryData()
    {
        if (hasLoadedData)
            return;
            
        discoveredMonsters.Clear();
        
        // Check if we have SimpleSaveSystem available
        if (SimpleSaveSystem.Instance != null)
        {
            // Get saved monster discoveries from save data
            Dictionary<string, MonsterDiscoveryStatus> savedMonsters = SimpleSaveSystem.Instance.GetBestiaryData();
            if (savedMonsters != null)
            {
                discoveredMonsters = savedMonsters;
            }
        }
        
        hasLoadedData = true;
    }
    
    /// <summary>
    /// Check if a monster has been discovered by the player
    /// </summary>
    public static bool IsMonsterDiscovered(string monsterId)
    {
        if (string.IsNullOrEmpty(monsterId))
            return false;
            
        return discoveredMonsters.ContainsKey(monsterId);
    }
    
    /// <summary>
    /// Check if a monster has been fully discovered (all details known)
    /// </summary>
    public static bool IsMonsterFullyDiscovered(string monsterId)
    {
        if (string.IsNullOrEmpty(monsterId) || !discoveredMonsters.ContainsKey(monsterId))
            return false;
            
        return discoveredMonsters[monsterId] == MonsterDiscoveryStatus.FullyDiscovered;
    }
    
    /// <summary>
    /// Record an encounter with a monster, marking it as discovered
    /// </summary>
    public static void RecordMonsterEncounter(string monsterId)
    {
        if (string.IsNullOrEmpty(monsterId))
            return;
            
        bool isNewDiscovery = !discoveredMonsters.ContainsKey(monsterId);
        
        // If the monster isn't already discovered, mark it as discovered
        if (isNewDiscovery)
        {
            discoveredMonsters[monsterId] = MonsterDiscoveryStatus.Basic;
            OnMonsterDiscoveryChanged?.Invoke(monsterId, MonsterDiscoveryStatus.Basic);
            
            // Save the updated bestiary data
            SaveBestiaryData();
        }
    }
    
    /// <summary>
    /// Mark a monster as fully discovered (player defeated it or analyzed it)
    /// </summary>
    public static void DiscoverMonsterFully(string monsterId)
    {
        if (string.IsNullOrEmpty(monsterId))
            return;
            
        bool isNewDiscovery = !discoveredMonsters.ContainsKey(monsterId);
        bool wasPartiallyDiscovered = discoveredMonsters.ContainsKey(monsterId) && 
                                      discoveredMonsters[monsterId] != MonsterDiscoveryStatus.FullyDiscovered;
        
        // Update the discovery status
        discoveredMonsters[monsterId] = MonsterDiscoveryStatus.FullyDiscovered;
        
        // Notify listeners if this is a new discovery or status change
        if (isNewDiscovery || wasPartiallyDiscovered)
        {
            OnMonsterDiscoveryChanged?.Invoke(monsterId, MonsterDiscoveryStatus.FullyDiscovered);
            
            // Save the updated bestiary data
            SaveBestiaryData();
        }
    }
    
    /// <summary>
    /// Get a list of all discovered monster IDs
    /// </summary>
    public static List<string> GetDiscoveredMonsterIds()
    {
        return discoveredMonsters.Keys.ToList();
    }
    
    /// <summary>
    /// Get a monster's discovery status
    /// </summary>
    public static MonsterDiscoveryStatus GetMonsterDiscoveryStatus(string monsterId)
    {
        if (string.IsNullOrEmpty(monsterId) || !discoveredMonsters.ContainsKey(monsterId))
            return MonsterDiscoveryStatus.Undiscovered;
            
        return discoveredMonsters[monsterId];
    }
    
    /// <summary>
    /// Get a reference to a monster's scriptable object by ID
    /// </summary>
    public static ScriptableMonster GetMonsterById(string monsterId)
    {
        if (instance == null || string.IsNullOrEmpty(monsterId))
            return null;
            
        return instance.allMonsters.FirstOrDefault(m => 
            m.GetMonsterData().monsterId == monsterId);
    }
    
    /// <summary>
    /// Save bestiary data to the save system
    /// </summary>
    private static void SaveBestiaryData()
    {
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.SaveBestiaryData(discoveredMonsters);
        }
    }
    
    /// <summary>
    /// Get all discovered monsters with their discovery status
    /// </summary>
    public static Dictionary<string, MonsterDiscoveryStatus> GetAllDiscoveredMonsters()
    {
        return new Dictionary<string, MonsterDiscoveryStatus>(discoveredMonsters);
    }
    
    /// <summary>
    /// Get a list of all monster scriptable objects
    /// </summary>
    public static List<ScriptableMonster> GetAllMonsters()
    {
        if (instance == null)
            return new List<ScriptableMonster>();
            
        return instance.allMonsters;
    }
}

/// <summary>
/// Represents the discovery status of a monster in the bestiary
/// </summary>
public enum MonsterDiscoveryStatus
{
    Undiscovered,    // Not yet seen by player
    Basic,           // Player has seen the monster
    FullyDiscovered  // Player has defeated or fully analyzed the monster
} 