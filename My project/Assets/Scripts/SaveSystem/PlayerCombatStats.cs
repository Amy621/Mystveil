using UnityEngine;

/// <summary>
/// Adapter component for saving player combat statistics
/// </summary>
public class PlayerCombatStats : MonoBehaviour
{
    private PlayerCharacter playerCharacter;
    
    private void Awake()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
        if (playerCharacter == null)
        {
            Debug.LogError("PlayerCombatStats requires a PlayerCharacter component", this);
            enabled = false;
            return;
        }
        
        // Register to the save system
        SimpleSaveSystem.Instance.OnSave += SaveCombatStats;
        SimpleSaveSystem.Instance.OnLoad += LoadCombatStats;
    }

    private void OnDestroy()
    {
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.OnSave -= SaveCombatStats;
            SimpleSaveSystem.Instance.OnLoad -= LoadCombatStats;
        }
    }

    public void SaveCombatStats(SimpleSaveData saveData)
    {
        if (playerCharacter != null && playerCharacter.GetPlayerStats() != null)
        {
            var stats = playerCharacter.GetPlayerStats();
            saveData.attackPoints = stats.ATK;
            saveData.defensePoints = stats.DEF;
            saveData.specialAttackPoints = stats.SPA;
            saveData.specialDefensePoints = stats.SPD;
            saveData.speed = stats.SPE;
            
            Debug.Log($"Saved player combat stats: ATK={saveData.attackPoints}, DEF={saveData.defensePoints}, " +
                      $"SP.ATK={saveData.specialAttackPoints}, SP.DEF={saveData.specialDefensePoints}, SPD={saveData.speed}");
        }
    }

    public void LoadCombatStats(SimpleSaveData saveData)
    {
        if (playerCharacter != null && playerCharacter.GetPlayerStats() != null)
        {
            Debug.Log($"Loaded player combat stats: ATK={saveData.attackPoints}, DEF={saveData.defensePoints}, " +
                      $"SP.ATK={saveData.specialAttackPoints}, SP.DEF={saveData.specialDefensePoints}, SPD={saveData.speed}");
            
            // For scriptable objects, you might want to update runtime variables
            // instead of modifying the SO directly
            // Apply combat stats to gameplay systems as needed
        }
    }
} 