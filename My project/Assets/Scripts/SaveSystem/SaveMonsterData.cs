using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Component responsible for saving and loading monster data through the SimpleSaveSystem.
/// Attach this to the same GameObject that has the SimpleSaveSystem.
/// </summary>
public class SaveMonsterData : MonoBehaviour
{
    // Reference to the CodexMonstersManager
    private CodexMonstersManager monstersManager;
    
    private void Start()
    {
        // Find the SimpleSaveSystem and register to its events
        SimpleSaveSystem saveSystem = GetComponent<SimpleSaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.OnSave += OnSave;
            saveSystem.OnLoad += OnLoad;
            Debug.Log("SaveMonsterData registered with SimpleSaveSystem");
        }
        else
        {
            Debug.LogError("SaveMonsterData requires a SimpleSaveSystem component on the same GameObject", this);
            enabled = false;
        }
        
        // Find the CodexMonstersManager
        EnchantedCodex codex = FindObjectOfType<EnchantedCodex>();
        if (codex != null)
        {
            // Attempt to find the monsters manager through the codex
            monstersManager = codex.GetComponentInChildren<CodexMonstersManager>(true);
            if (monstersManager != null)
            {
                Debug.Log("SaveMonsterData found CodexMonstersManager");
            }
        }
        
        if (monstersManager == null)
        {
            // Fallback: try to find it directly in the scene
            monstersManager = FindObjectOfType<CodexMonstersManager>(true);
            if (monstersManager != null)
            {
                Debug.Log("SaveMonsterData found CodexMonstersManager through scene search");
            }
            else
            {
                Debug.LogWarning("SaveMonsterData could not find a CodexMonstersManager. Monster data will not be saved.");
            }
        }
    }
    
    private void OnDestroy()
    {
        // Unregister from the save system events
        SimpleSaveSystem saveSystem = GetComponent<SimpleSaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.OnSave -= OnSave;
            saveSystem.OnLoad -= OnLoad;
        }
    }
    
    // Called when the game is being saved
    private void OnSave(SimpleSaveData saveData)
    {
        if (monstersManager == null)
        {
            Debug.LogWarning("SaveMonsterData: Cannot save monster data - CodexMonstersManager not found");
            return;
        }
        
        // Let the CodexMonstersManager handle the actual saving
        monstersManager.OnSave(saveData);
    }
    
    // Called when the game is being loaded
    private void OnLoad(SimpleSaveData saveData)
    {
        if (monstersManager == null)
        {
            Debug.LogWarning("SaveMonsterData: Cannot load monster data - CodexMonstersManager not found");
            return;
        }
        
        // Let the CodexMonstersManager handle the actual loading
        monstersManager.OnLoad(saveData);
    }
} 