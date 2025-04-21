using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Extension methods for SimpleSaveSystem to handle bestiary data.
/// This provides the bridge between SimpleSaveSystem and CodexBestiaryManager.
/// </summary>
public static class SimpleSaveBestiaryExtension
{
    private const string BESTIARY_PREFIX = "bestiary_";
    private const string MONSTERS_KEY = BESTIARY_PREFIX + "monsters";
    
    /// <summary>
    /// Save bestiary data to the SimpleSaveData.
    /// </summary>
    public static void SaveBestiaryData(this SimpleSaveSystem saveSystem, Dictionary<string, MonsterDiscoveryStatus> discoveredMonsters)
    {
        // This method should run when saving the game
        saveSystem.OnSave += (saveData) => 
        {
            // Save each monster's discovery status
            foreach (var kvp in discoveredMonsters)
            {
                string monsterId = kvp.Key;
                MonsterDiscoveryStatus status = kvp.Value;
                
                // Save as an integer value
                saveData.SetInt(BESTIARY_PREFIX + monsterId, (int)status);
            }
            
            // Save list of all discovered monster IDs
            string[] monsterIds = new string[discoveredMonsters.Count];
            int index = 0;
            foreach (string id in discoveredMonsters.Keys)
            {
                monsterIds[index++] = id;
            }
            
            saveData.SetStringArray(MONSTERS_KEY, monsterIds);
            
            Debug.Log($"Saved bestiary data with {discoveredMonsters.Count} monsters");
        };
    }
    
    /// <summary>
    /// Get bestiary data from the most recent save.
    /// </summary>
    public static Dictionary<string, MonsterDiscoveryStatus> GetBestiaryData(this SimpleSaveSystem saveSystem)
    {
        Dictionary<string, MonsterDiscoveryStatus> result = new Dictionary<string, MonsterDiscoveryStatus>();
        
        // Create a local variable to store data when loaded
        Dictionary<string, MonsterDiscoveryStatus> loadedData = null;
        
        // Register a one-time handler to extract data during next load
        saveSystem.OnLoad += ExtractBestiaryData;
        
        void ExtractBestiaryData(SimpleSaveData saveData)
        {
            // Get list of monster IDs
            string[] monsterIds = saveData.GetStringArray(MONSTERS_KEY);
            
            if (monsterIds != null && monsterIds.Length > 0)
            {
                loadedData = new Dictionary<string, MonsterDiscoveryStatus>();
                
                // Load each monster's discovery status
                foreach (string id in monsterIds)
                {
                    int statusValue = saveData.GetInt(BESTIARY_PREFIX + id, 0);
                    MonsterDiscoveryStatus status = (MonsterDiscoveryStatus)statusValue;
                    loadedData[id] = status;
                }
                
                Debug.Log($"Loaded bestiary data with {loadedData.Count} monsters");
            }
            else
            {
                loadedData = new Dictionary<string, MonsterDiscoveryStatus>();
                Debug.Log("No bestiary data found in save file");
            }
            
            // Unregister the handler
            saveSystem.OnLoad -= ExtractBestiaryData;
        }
        
        // Try to load the save if it exists
        if (System.IO.File.Exists(saveSystem.SaveFilePath))
        {
            saveSystem.LoadGame();
            
            // Wait a bit to ensure load is complete
            if (loadedData != null)
            {
                result = loadedData;
            }
        }
        
        return result;
    }
} 