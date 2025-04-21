using UnityEngine;

/// <summary>
/// Component responsible for loading the monster database and initializing
/// the monster catalog at game start.
/// </summary>
public class MonsterDatabaseLoader : MonoBehaviour
{
    [SerializeField] private MonsterDatabase monsterDatabase;
    [SerializeField] private bool logDebugInfo = true;
    
    private void Awake()
    {
        if (monsterDatabase == null)
        {
            Debug.LogError("Monster Database reference is missing!", this);
            return;
        }
        
        // Convert monster data and initialize the catalog
        var codexMonsterData = monsterDatabase.GetCodexMonsterData();
        CodexMonstersManager.MonsterCatalog.Initialize(codexMonsterData);
        
        if (logDebugInfo)
        {
            Debug.Log($"MonsterDatabaseLoader: Initialized catalog with {codexMonsterData.Count} monsters");
        }
    }
    
    /// <summary>
    /// Helper method to discover a monster by ID.
    /// This can be called from other scripts or via UnityEvent.
    /// </summary>
    public void DiscoverMonster(string monsterId)
    {
        MonsterDiscoveryHelper.DiscoverMonster(monsterId);
    }
    
    /// <summary>
    /// Helper method to discover a monster when the player enters a new area.
    /// Call this from area triggers.
    /// </summary>
    public void DiscoverAreaMonsters(string areaName)
    {
        // Get all monsters that can appear in this area
        var allMonsters = monsterDatabase.GetAllMonsters();
        int count = 0;
        
        foreach (var monster in allMonsters)
        {
            // Check if this monster appears in the given area
            if (monster.locations.Contains(areaName))
            {
                if (MonsterDiscoveryHelper.DiscoverMonster(monster.id))
                {
                    count++;
                }
            }
        }
        
        if (count > 0 && logDebugInfo)
        {
            Debug.Log($"Discovered {count} new monsters in area: {areaName}");
        }
    }
} 