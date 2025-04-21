using UnityEngine;

/// <summary>
/// Static helper class to make it easy to discover monsters from anywhere in the game.
/// This class provides a simple API for marking monsters as discovered for the Codex/Bestiary.
/// </summary>
public static class MonsterDiscoveryHelper
{
    /// <summary>
    /// Discovers a monster by its ID and adds it to the Codex/Bestiary.
    /// </summary>
    /// <param name="monsterId">The unique ID of the monster to discover</param>
    /// <returns>True if the monster was newly discovered, false if already known</returns>
    public static bool DiscoverMonster(string monsterId)
    {
        bool wasNewDiscovery = false;
        
        // Notify any listeners through the MonsterCatalog
        // This will eventually reach the CodexMonstersManager
        if (CodexMonstersManager.MonsterCatalog.DiscoverMonster(monsterId))
        {
            wasNewDiscovery = true;
            Debug.Log($"Monster discovered: {monsterId}");
            
            // Auto-save the game when a new monster is discovered
            if (SimpleSaveSystem.Instance != null)
            {
                SimpleSaveSystem.Instance.SaveGame();
            }
        }
        
        return wasNewDiscovery;
    }
    
    /// <summary>
    /// Checks if a monster has been discovered.
    /// </summary>
    /// <param name="monsterId">The unique ID of the monster to check</param>
    /// <returns>True if the monster has been discovered, false otherwise</returns>
    public static bool IsMonsterDiscovered(string monsterId)
    {
        return CodexMonstersManager.MonsterCatalog.IsMonsterDiscovered(monsterId);
    }
    
    /// <summary>
    /// Gets the count of discovered monsters.
    /// </summary>
    /// <returns>The number of monsters discovered</returns>
    public static int GetDiscoveredMonsterCount()
    {
        return CodexMonstersManager.MonsterCatalog.GetDiscoveredMonsterCount();
    }
    
    /// <summary>
    /// Gets the total number of discoverable monsters in the game.
    /// </summary>
    /// <returns>The total number of monsters in the monster catalog</returns>
    public static int GetTotalMonsterCount()
    {
        return CodexMonstersManager.MonsterCatalog.GetTotalMonsterCount();
    }
} 