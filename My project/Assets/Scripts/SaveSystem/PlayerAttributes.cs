using UnityEngine;

/// <summary>
/// Adapter component for saving player charisma and other non-combat attributes
/// </summary>
public class PlayerAttributes : MonoBehaviour
{
    private PlayerCharacter playerCharacter;
    
    private void Awake()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
        if (playerCharacter == null)
        {
            Debug.LogError("PlayerAttributes requires a PlayerCharacter component", this);
            enabled = false;
            return;
        }
        
        // Register to the save system
        SimpleSaveSystem.Instance.OnSave += SaveAttributes;
        SimpleSaveSystem.Instance.OnLoad += LoadAttributes;
    }

    private void OnDestroy()
    {
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.OnSave -= SaveAttributes;
            SimpleSaveSystem.Instance.OnLoad -= LoadAttributes;
        }
    }

    public void SaveAttributes(SimpleSaveData saveData)
    {
        if (playerCharacter != null && playerCharacter.GetPlayerStats() != null)
        {
            saveData.charisma = playerCharacter.GetPlayerStats().CHA;
            Debug.Log($"Saved player charisma: {saveData.charisma}");
        }
    }

    public void LoadAttributes(SimpleSaveData saveData)
    {
        if (playerCharacter != null && playerCharacter.GetPlayerStats() != null)
        {
            // For scriptable objects, you might want to update a runtime variable 
            // instead of modifying the SO directly
            Debug.Log($"Loaded player charisma: {saveData.charisma}");
            // Apply charisma value to gameplay systems as needed
        }
    }
} 