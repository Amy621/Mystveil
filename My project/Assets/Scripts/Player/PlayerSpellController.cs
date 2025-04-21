using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the player's spells, allowing them to equip, cast, and manage spell cooldowns.
/// This is referenced by the CodexSpellsManager for equipment.
/// </summary>
public class PlayerSpellController : MonoBehaviour
{
    [SerializeField] private int maxEquippedSpells = 4;
    
    // Currently equipped spells
    private string[] equippedSpells;
    
    // Dictionary of all known spells
    private Dictionary<string, SpellData> knownSpells = new Dictionary<string, SpellData>();
    
    // Delegates for events
    public delegate void SpellsChangedHandler();
    public event SpellsChangedHandler OnSpellsChanged;
    
    private void Awake()
    {
        // Initialize equipped spells array
        equippedSpells = new string[maxEquippedSpells];
    }
    
    /// <summary>
    /// Learn a new spell and add it to the player's known spells.
    /// </summary>
    public void LearnSpell(SpellData spell)
    {
        if (spell != null && !knownSpells.ContainsKey(spell.spellId))
        {
            knownSpells[spell.spellId] = spell;
            Debug.Log($"Learned new spell: {spell.spellName}");
            
            // Notify listeners
            OnSpellsChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// Updates the equipped spells array from external sources.
    /// </summary>
    public void UpdateEquippedSpells(string[] spellIds)
    {
        if (spellIds == null || spellIds.Length != equippedSpells.Length)
        {
            Debug.LogError("Invalid spell array provided to UpdateEquippedSpells");
            return;
        }
        
        // Copy the array
        for (int i = 0; i < spellIds.Length; i++)
        {
            equippedSpells[i] = spellIds[i];
        }
        
        // Notify listeners
        OnSpellsChanged?.Invoke();
    }
    
    /// <summary>
    /// Gets a list of all known spells.
    /// </summary>
    public List<SpellData> GetKnownSpells()
    {
        return new List<SpellData>(knownSpells.Values);
    }
    
    /// <summary>
    /// Gets a list of all equipped spell IDs.
    /// </summary>
    public string[] GetEquippedSpellIds()
    {
        return equippedSpells;
    }
    
    /// <summary>
    /// Attempt to cast a spell in a specific slot.
    /// </summary>
    public bool CastSpell(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedSpells.Length)
        {
            return false;
        }
        
        string spellId = equippedSpells[slotIndex];
        if (string.IsNullOrEmpty(spellId))
        {
            Debug.Log("No spell equipped in slot " + slotIndex);
            return false;
        }
        
        if (knownSpells.TryGetValue(spellId, out SpellData spell))
        {
            Debug.Log($"Casting spell: {spell.spellName}");
            // Here you would add the actual casting logic
            return true;
        }
        
        return false;
    }
}

/// <summary>
/// Data structure for spell information.
/// </summary>
[System.Serializable]
public class SpellData
{
    public string spellId;
    public string spellName;
    public string spellType;  // Fire, Ice, Lightning, etc.
    public string description;
    public int manaCost;
    public float cooldownTime;
    public Sprite icon;
} 