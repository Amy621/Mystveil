using UnityEngine;

/// <summary>
/// Adapter component for saving player resource statistics (health and mana)
/// </summary>
public class PlayerResourceStats : MonoBehaviour
{
    private PlayerCharacter playerCharacter;
    
    private void Awake()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
        if (playerCharacter == null)
        {
            Debug.LogError("PlayerResourceStats requires a PlayerCharacter component", this);
            enabled = false;
            return;
        }
        
        // Register to the save system
        SimpleSaveSystem.Instance.OnSave += SaveResourceStats;
        SimpleSaveSystem.Instance.OnLoad += LoadResourceStats;
    }

    private void OnDestroy()
    {
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.OnSave -= SaveResourceStats;
            SimpleSaveSystem.Instance.OnLoad -= LoadResourceStats;
        }
    }

    public void SaveResourceStats(SimpleSaveData saveData)
    {
        if (playerCharacter != null && playerCharacter.GetPlayerStats() != null)
        {
            var stats = playerCharacter.GetPlayerStats();
            // Save current and max values for health and mana
            saveData.health = stats.HP;
            saveData.maxHealth = stats.HP; // Using HP for both since there's no separate maxHP
            saveData.mana = stats.MANA;
            saveData.maxMana = stats.MANA; // Using MANA for both since there's no separate maxMANA
            
            Debug.Log($"Saved player resources: HP={saveData.health}/{saveData.maxHealth}, " +
                      $"MP={saveData.mana}/{saveData.maxMana}");
        }
    }

    public void LoadResourceStats(SimpleSaveData saveData)
    {
        if (playerCharacter != null && playerCharacter.GetPlayerStats() != null)
        {
            var stats = playerCharacter.GetPlayerStats();
            
            // Apply the loaded values to the player's current stats
            Debug.Log($"Loaded player resources: HP={saveData.health}/{saveData.maxHealth}, " +
                      $"MP={saveData.mana}/{saveData.maxMana}");
            
            // For scriptable objects, you might want to update runtime variables
            // instead of modifying the SO directly
            // Apply resource values to gameplay systems as needed
        }
    }
} 