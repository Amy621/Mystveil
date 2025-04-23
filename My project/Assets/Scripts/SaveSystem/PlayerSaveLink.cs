using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Links the Player class from Player.cs to the save system
/// This component should be attached to the player GameObject
/// </summary>
public class PlayerSaveLink : MonoBehaviour
{
    [Tooltip("Reference to the Player instance if not obtained through code")]
    [SerializeField] private Player playerReference;
    
    [Tooltip("Optional field to find a MonoBehaviour that can provide the Player instance")]
    [SerializeField] private MonoBehaviour playerProvider;
    
    private Player player;
    private SpellManager spellManager;
    
    private void Awake()
    {
        // Use the direct reference if provided
        player = playerReference;
        
        // If no direct reference, try to find it elsewhere
        if (player == null)
        {
            // Try to get it from a serialize field provider
            if (playerProvider != null)
            {
                // Try using reflection to find a GetPlayer() method
                var method = playerProvider.GetType().GetMethod("GetPlayer");
                if (method != null)
                {
                    player = method.Invoke(playerProvider, null) as Player;
                }
                
                // Try using reflection to find a 'player' field
                if (player == null)
                {
                    var field = playerProvider.GetType().GetField("player", 
                        System.Reflection.BindingFlags.Public | 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    
                    if (field != null)
                    {
                        player = field.GetValue(playerProvider) as Player;
                    }
                }
            }
            
            // If still no player, look through common components
            if (player == null)
            {
                // Find potential player container components
                var potentialContainers = FindObjectsOfType<MonoBehaviour>();
                foreach (var container in potentialContainers)
                {
                    // Look for a GetPlayer method
                    var method = container.GetType().GetMethod("GetPlayer");
                    if (method != null)
                    {
                        player = method.Invoke(container, null) as Player;
                        if (player != null) break;
                    }
                    
                    // Look for a player field/property
                    var playerProperty = container.GetType().GetProperty("player");
                    if (playerProperty != null)
                    {
                        player = playerProperty.GetValue(container) as Player;
                        if (player != null) break;
                    }
                    
                    // Check for fields named player or currentPlayer
                    var playerField = container.GetType().GetField("player", 
                        System.Reflection.BindingFlags.Public | 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    
                    if (playerField != null)
                    {
                        player = playerField.GetValue(container) as Player;
                        if (player != null) break;
                    }
                    
                    var currentPlayerField = container.GetType().GetField("currentPlayer", 
                        System.Reflection.BindingFlags.Public | 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    
                    if (currentPlayerField != null)
                    {
                        player = currentPlayerField.GetValue(container) as Player;
                        if (player != null) break;
                    }
                }
            }
        }
        
        if (player == null)
        {
            Debug.LogWarning("PlayerSaveLink couldn't find a Player instance. Saving player data won't work.", this);
        }
        else
        {
            Debug.Log("PlayerSaveLink found Player instance", this);
        }
        
        // Get the SpellManager
        spellManager = GetComponent<SpellManager>();
        if (spellManager == null)
        {
            spellManager = FindObjectOfType<SpellManager>();
        }
        
        // Register to save system events
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.OnSave += SavePlayerData;
            SimpleSaveSystem.Instance.OnLoad += LoadPlayerData;
            
            Debug.Log("PlayerSaveLink connected to save system");
        }
        else
        {
            Debug.LogWarning("SimpleSaveSystem instance not found. Player data won't be saved/loaded.");
        }
    }
    
    private void OnDestroy()
    {
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.OnSave -= SavePlayerData;
            SimpleSaveSystem.Instance.OnLoad -= LoadPlayerData;
        }
    }
    
    public void SavePlayerData(SimpleSaveData saveData)
    {
        if (player != null)
        {
            // Save basic stats
            saveData.health = player.HP;
            saveData.maxHealth = player.MaxHp;
            saveData.mana = player.MANA;
            saveData.maxMana = player.MaxMana;
            saveData.level = player.Level;
            
            // Save combat stats
            saveData.attackPoints = player.Attack;
            saveData.defensePoints = player.Defense;
            saveData.specialAttackPoints = player.SpAttack;
            saveData.specialDefensePoints = player.SpDefense;
            saveData.speed = player.Speed;
            
            // Save charisma
            saveData.charisma = player.Charisma;
            
            Debug.Log($"Saved player data: HP={saveData.health}/{saveData.maxHealth}, " +
                      $"MP={saveData.mana}/{saveData.maxMana}, Level={saveData.level}");
                      
            // NEW IMPROVED SPELL SAVING CODE
            if (player.Spells != null)
            {
                // Initialize lists if they're null
                if (saveData.unlockedSpells == null)
                    saveData.unlockedSpells = new List<string>();
                else
                    saveData.unlockedSpells.Clear();
                    
                if (saveData.equippedSpells == null)
                    saveData.equippedSpells = new List<string>();
                else
                    saveData.equippedSpells.Clear();
                
                Debug.Log($"Player has {player.Spells.Count} spells to save");
                
                // Process each spell
                foreach (var spell in player.Spells)
                {
                    if (spell != null && spell.Base != null)
                    {
                        // Get a unique identifier for the spell
                        // Try multiple properties that might contain the ID
                        string spellId = null;
                        
                        // Try SpellID property
                        var idProperty = spell.Base.GetType().GetProperty("SpellID");
                        if (idProperty != null)
                            spellId = idProperty.GetValue(spell.Base) as string;
                        
                        // Try ID property
                        if (string.IsNullOrEmpty(spellId))
                        {
                            idProperty = spell.Base.GetType().GetProperty("ID");
                            if (idProperty != null)
                                spellId = idProperty.GetValue(spell.Base) as string;
                        }
                        
                        // Try moveBase.name as fallback
                        if (string.IsNullOrEmpty(spellId))
                            spellId = spell.Base.name;
                            
                        // If we still don't have an ID, try the spell name
                        if (string.IsNullOrEmpty(spellId))
                        {
                            var nameProperty = spell.Base.GetType().GetProperty("Name");
                            if (nameProperty != null)
                                spellId = nameProperty.GetValue(spell.Base) as string;
                        }
                            
                        // As a last resort, use ToString
                        if (string.IsNullOrEmpty(spellId))
                            spellId = spell.Base.ToString();
                        
                        // Add to unlocked spells (all spells the player has are unlocked)
                        if (!string.IsNullOrEmpty(spellId) && !saveData.unlockedSpells.Contains(spellId))
                        {
                            saveData.unlockedSpells.Add(spellId);
                            Debug.Log($"Saved unlocked spell: {spellId}");
                        }
                        
                        // Add to equipped spells (assuming all spells in player.Spells are equipped)
                        if (!string.IsNullOrEmpty(spellId) && !saveData.equippedSpells.Contains(spellId))
                        {
                            saveData.equippedSpells.Add(spellId);
                            Debug.Log($"Saved equipped spell: {spellId}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Skipped null spell or spell with null base");
                    }
                }
                
                Debug.Log($"Saved player spells: {saveData.unlockedSpells.Count} unlocked, {saveData.equippedSpells.Count} equipped");
            }
            else
            {
                Debug.LogWarning("Player.Spells is null, can't save spell data");
            }
        }
        
        // Check if SpellManager can provide additional spells
        if (spellManager != null && saveData.unlockedSpells.Count == 0)
        {
            var managerUnlocked = spellManager.GetUnlockedSpells();
            var managerEquipped = spellManager.GetEquippedSpells();
            
            if (managerUnlocked != null && managerUnlocked.Count > 0)
            {
                saveData.unlockedSpells = managerUnlocked;
                Debug.Log($"Used SpellManager to save {managerUnlocked.Count} unlocked spells");
            }
            
            if (managerEquipped != null && managerEquipped.Count > 0)
            {
                saveData.equippedSpells = managerEquipped;
                Debug.Log($"Used SpellManager to save {managerEquipped.Count} equipped spells");
            }
        }
        
        // Final report on spell saving
        Debug.Log($"Final spell save count: {saveData.unlockedSpells?.Count ?? 0} unlocked, {saveData.equippedSpells?.Count ?? 0} equipped");
    }
    
    public void LoadPlayerData(SimpleSaveData saveData)
    {
        if (player != null)
        {
            // Load basic stats
            player.HP = saveData.health;
            player.MANA = saveData.mana;
            // We don't update MaxHp and MaxMana as those are typically calculated from level and base stats
            
            Debug.Log($"Loaded player data: HP={saveData.health}/{saveData.maxHealth}, " +
                      $"MP={saveData.mana}/{saveData.maxMana}, Level={saveData.level}");
                      
            // We might need to handle spell loading later if the systems change
        }
        
        // SpellManager will handle loading spells if available
    }
} 