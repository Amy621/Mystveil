using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private SpellDatabase spellDatabase;
    [SerializeField] private int maxEquippedSpells = 8;
    
    private List<string> unlockedSpells = new List<string>();
    private List<string> equippedSpells = new List<string>();
    
    public delegate void SpellsChangedEvent();
    public event SpellsChangedEvent OnSpellsChanged;
    
    // Unlock a new spell
    public bool UnlockSpell(string spellID)
    {
        if (unlockedSpells.Contains(spellID))
        {
            Debug.Log($"Spell {spellID} already unlocked");
            return false;
        }
        
        // Check if spell exists in database
        Spell spell = spellDatabase.GetSpellByID(spellID);
        if (spell == null)
        {
            Debug.LogError($"Spell with ID {spellID} not found in database");
            return false;
        }
        
        unlockedSpells.Add(spellID);
        Debug.Log($"Unlocked spell: {spell.SpellName}");
        OnSpellsChanged?.Invoke();
        
        return true;
    }
    
    // Equip a spell
    public bool EquipSpell(string spellID)
    {
        if (!unlockedSpells.Contains(spellID))
        {
            Debug.Log($"Cannot equip spell {spellID} - not unlocked");
            return false;
        }
        
        if (equippedSpells.Contains(spellID))
        {
            Debug.Log($"Spell {spellID} already equipped");
            return false;
        }
        
        if (equippedSpells.Count >= maxEquippedSpells)
        {
            Debug.Log("Cannot equip more spells - maximum reached");
            return false;
        }
        
        equippedSpells.Add(spellID);
        Debug.Log($"Equipped spell: {spellDatabase.GetSpellByID(spellID).SpellName}");
        OnSpellsChanged?.Invoke();
        
        return true;
    }
    
    // Unequip a spell
    public bool UnequipSpell(string spellID)
    {
        if (!equippedSpells.Contains(spellID))
        {
            Debug.Log($"Spell {spellID} not equipped");
            return false;
        }
        
        equippedSpells.Remove(spellID);
        Debug.Log($"Unequipped spell: {spellDatabase.GetSpellByID(spellID).SpellName}");
        OnSpellsChanged?.Invoke();
        
        return true;
    }
    
    // Equip spell to specific slot (replace if occupied)
    public bool EquipSpellToSlot(string spellID, int slotIndex)
    {
        if (!unlockedSpells.Contains(spellID))
        {
            Debug.Log($"Cannot equip spell {spellID} - not unlocked");
            return false;
        }
        
        if (slotIndex < 0 || slotIndex >= maxEquippedSpells)
        {
            Debug.Log($"Invalid spell slot index: {slotIndex}");
            return false;
        }
        
        // Remove from current position if already equipped
        if (equippedSpells.Contains(spellID))
        {
            equippedSpells.Remove(spellID);
        }
        
        // Expand equipped spells list if needed
        while (equippedSpells.Count <= slotIndex)
        {
            equippedSpells.Add(null);
        }
        
        // Assign to slot
        equippedSpells[slotIndex] = spellID;
        Debug.Log($"Equipped spell {spellDatabase.GetSpellByID(spellID).SpellName} to slot {slotIndex}");
        OnSpellsChanged?.Invoke();
        
        return true;
    }
    
    // Get all unlocked spells
    public List<Spell> GetUnlockedSpellsList()
    {
        List<Spell> spells = new List<Spell>();
        
        foreach (string spellID in unlockedSpells)
        {
            Spell spell = spellDatabase.GetSpellByID(spellID);
            if (spell != null)
            {
                spells.Add(spell);
            }
        }
        
        return spells;
    }
    
    // Get all equipped spells
    public List<Spell> GetEquippedSpellsList()
    {
        List<Spell> spells = new List<Spell>();
        
        foreach (string spellID in equippedSpells)
        {
            if (!string.IsNullOrEmpty(spellID))
            {
                Spell spell = spellDatabase.GetSpellByID(spellID);
                if (spell != null)
                {
                    spells.Add(spell);
                }
            }
            else
            {
                spells.Add(null); // Empty slot
            }
        }
        
        return spells;
    }
    
    // Check if spell is unlocked
    public bool IsSpellUnlocked(string spellID)
    {
        return unlockedSpells.Contains(spellID);
    }
    
    // Check if spell is equipped
    public bool IsSpellEquipped(string spellID)
    {
        return equippedSpells.Contains(spellID);
    }
    
    // Get spell by ID
    public Spell GetSpell(string spellID)
    {
        return spellDatabase.GetSpellByID(spellID);
    }
    
    // Get all spells from the database
    public List<Spell> GetAllSpells()
    {
        return spellDatabase.GetAllSpells();
    }
    
    // Get maximum number of equipped spells
    public int GetMaxEquippedSpells()
    {
        return maxEquippedSpells;
    }
    
    // Get the equipped spell in a specific slot
    public Spell GetSpellInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedSpells.Count)
        {
            return null;
        }
        
        string spellID = equippedSpells[slotIndex];
        if (string.IsNullOrEmpty(spellID))
        {
            return null;
        }
        
        return spellDatabase.GetSpellByID(spellID);
    }
    
    // Cast a spell by ID
    public bool CastSpell(string spellID)
    {
        if (!IsSpellEquipped(spellID))
        {
            Debug.Log($"Cannot cast spell {spellID} - not equipped");
            return false;
        }
        
        Spell spell = spellDatabase.GetSpellByID(spellID);
        if (spell != null)
        {
            // Cast the spell logic would go here
            Debug.Log($"Casting spell: {spell.SpellName}");
            return true;
        }
        
        return false;
    }
    
    // Cast a spell by slot
    public bool CastSpellInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedSpells.Count)
        {
            Debug.Log($"Invalid spell slot index: {slotIndex}");
            return false;
        }
        
        string spellID = equippedSpells[slotIndex];
        if (string.IsNullOrEmpty(spellID))
        {
            Debug.Log($"No spell equipped in slot {slotIndex}");
            return false;
        }
        
        return CastSpell(spellID);
    }
    
    // For save system - get unlocked spell IDs
    public List<string> GetUnlockedSpells()
    {
        return new List<string>(unlockedSpells);
    }
    
    // For save system - get equipped spell IDs
    public List<string> GetEquippedSpells()
    {
        return new List<string>(equippedSpells);
    }
    
    // For save system - load unlocked spells
    public void LoadUnlockedSpells(List<string> spellIDs)
    {
        unlockedSpells = new List<string>(spellIDs);
        OnSpellsChanged?.Invoke();
    }
    
    // For save system - load equipped spells
    public void LoadEquippedSpells(List<string> spellIDs)
    {
        equippedSpells = new List<string>(spellIDs);
        OnSpellsChanged?.Invoke();
    }
    
    // Reset to default state
    public void ResetToDefault()
    {
        unlockedSpells.Clear();
        equippedSpells.Clear();
        
        // Unlock and equip basic starting spells
        string[] startingSpells = new string[] { "spell_fireball_basic", "spell_heal_minor" };
        
        foreach (string spellID in startingSpells)
        {
            if (spellDatabase.GetSpellByID(spellID) != null)
            {
                UnlockSpell(spellID);
                EquipSpell(spellID);
            }
        }
        
        OnSpellsChanged?.Invoke();
    }
} 