using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Singleton class that manages the game's monster catalog.
/// Provides access to all monster data whether discovered or not.
/// </summary>
public class MonsterCatalog
{
    // Singleton instance
    private static MonsterCatalog _instance;
    
    // All monsters in the game
    private Dictionary<string, CodexMonstersManager.MonsterData> _allMonsters = new Dictionary<string, CodexMonstersManager.MonsterData>();
    
    // Set of discovered monster IDs
    private HashSet<string> _discoveredMonsterIds = new HashSet<string>();
    
    // Event delegate for monster discovery
    public delegate void MonsterDiscoveryEvent(string monsterId);
    public event MonsterDiscoveryEvent OnMonsterDiscovered;
    
    // Static accessor for the monster catalog
    public static MonsterCatalog Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MonsterCatalog();
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// Initializes the monster catalog with the given monster data.
    /// </summary>
    public void Initialize(List<CodexMonstersManager.MonsterData> monsters)
    {
        _allMonsters.Clear();
        
        foreach (var monster in monsters)
        {
            _allMonsters[monster.monsterId] = monster;
        }
        
        Debug.Log($"MonsterCatalog initialized with {_allMonsters.Count} monsters");
    }
    
    /// <summary>
    /// Initializes the monster catalog with monsters from Resources.
    /// </summary>
    public void InitializeFromResources()
    {
        _allMonsters.Clear();
        
        ScriptableMonster[] monsters = Resources.LoadAll<ScriptableMonster>("1V1/Monsters");
        
        if (monsters != null && monsters.Length > 0)
        {
            foreach (var monster in monsters)
            {
                CodexMonstersManager.MonsterData monsterData = monster.GetMonsterData();
                _allMonsters[monsterData.monsterId] = monsterData;
            }
            
            Debug.Log($"MonsterCatalog initialized with {_allMonsters.Count} monsters from Resources");
        }
        else
        {
            Debug.LogWarning("No monsters found in Resources/1V1/Monsters");
        }
    }
    
    /// <summary>
    /// Gets a monster by its ID.
    /// </summary>
    public CodexMonstersManager.MonsterData GetMonster(string monsterId)
    {
        if (_allMonsters.TryGetValue(monsterId, out var monster))
        {
            return monster;
        }
        return null;
    }
    
    /// <summary>
    /// Gets all monsters in the catalog.
    /// </summary>
    public List<CodexMonstersManager.MonsterData> GetAllMonsters()
    {
        return new List<CodexMonstersManager.MonsterData>(_allMonsters.Values);
    }
    
    /// <summary>
    /// Gets all discovered monsters.
    /// </summary>
    public List<CodexMonstersManager.MonsterData> GetDiscoveredMonsters()
    {
        return _allMonsters
            .Where(kvp => _discoveredMonsterIds.Contains(kvp.Key))
            .Select(kvp => kvp.Value)
            .ToList();
    }
    
    /// <summary>
    /// Discovers a monster by ID.
    /// </summary>
    /// <returns>True if newly discovered, false if already known</returns>
    public bool DiscoverMonster(string monsterId)
    {
        if (!_allMonsters.ContainsKey(monsterId))
        {
            Debug.LogWarning($"Attempted to discover unknown monster: {monsterId}");
            return false;
        }
        
        if (_discoveredMonsterIds.Contains(monsterId))
        {
            return false; // Already discovered
        }
        
        _discoveredMonsterIds.Add(monsterId);
        OnMonsterDiscovered?.Invoke(monsterId);
        
        return true;
    }
    
    /// <summary>
    /// Checks if a monster has been discovered.
    /// </summary>
    public bool IsMonsterDiscovered(string monsterId)
    {
        return _discoveredMonsterIds.Contains(monsterId);
    }
    
    /// <summary>
    /// Resets all monster discoveries.
    /// </summary>
    public void ResetDiscoveries()
    {
        _discoveredMonsterIds.Clear();
    }
    
    /// <summary>
    /// Sets the list of discovered monster IDs.
    /// </summary>
    public void SetDiscoveredMonsters(IEnumerable<string> monsterIds)
    {
        _discoveredMonsterIds.Clear();
        foreach (var id in monsterIds)
        {
            if (_allMonsters.ContainsKey(id))
            {
                _discoveredMonsterIds.Add(id);
            }
        }
    }
    
    /// <summary>
    /// Gets the count of discovered monsters.
    /// </summary>
    public int GetDiscoveredMonsterCount()
    {
        return _discoveredMonsterIds.Count;
    }
    
    /// <summary>
    /// Gets the total number of monsters in the catalog.
    /// </summary>
    public int GetTotalMonsterCount()
    {
        return _allMonsters.Count;
    }
} 