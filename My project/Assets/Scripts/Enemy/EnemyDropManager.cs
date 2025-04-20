using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropManager : MonoBehaviour
{
    public static EnemyDropManager Instance { get; private set; }
    
    // Dictionary to track drop stats for each enemy type
    private Dictionary<string, float> enemyDropStats = new Dictionary<string, float>();
    
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
    
    // Record a drop from an enemy
    public void RecordEnemyDrop(string enemyID, float dropValue)
    {
        if (string.IsNullOrEmpty(enemyID))
        {
            Debug.LogError("Cannot record drop with null enemy ID");
            return;
        }
        
        // If enemy exists in dictionary, update value
        if (enemyDropStats.ContainsKey(enemyID))
        {
            enemyDropStats[enemyID] += dropValue;
        }
        else
        {
            // Otherwise, add new entry
            enemyDropStats.Add(enemyID, dropValue);
        }
        
        Debug.Log($"Recorded drop for {enemyID}, value: {dropValue}, total: {enemyDropStats[enemyID]}");
        
        // Trigger save after recording drop
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.SaveGame();
        }
    }
    
    // Get total drops from a specific enemy type
    public float GetEnemyTotalDrops(string enemyID)
    {
        if (enemyDropStats.ContainsKey(enemyID))
        {
            return enemyDropStats[enemyID];
        }
        
        return 0f;
    }
    
    // Get all tracked enemy IDs
    public List<string> GetTrackedEnemies()
    {
        return new List<string>(enemyDropStats.Keys);
    }
    
    // Clear drop stats for a specific enemy
    public void ClearEnemyDropStats(string enemyID)
    {
        if (enemyDropStats.ContainsKey(enemyID))
        {
            enemyDropStats.Remove(enemyID);
        }
    }
    
    // Clear all drop stats
    public void ClearAllDropStats()
    {
        enemyDropStats.Clear();
    }
    
    // Get drop stats for saving
    public Dictionary<string, float> GetEnemyDropStats()
    {
        return new Dictionary<string, float>(enemyDropStats);
    }
    
    // Load drop stats
    public void LoadEnemyDropStats(Dictionary<string, float> stats)
    {
        enemyDropStats = new Dictionary<string, float>(stats);
    }
} 