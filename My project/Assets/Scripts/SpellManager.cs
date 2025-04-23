using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Component that manages the player's spells
/// </summary>
public class SpellManager : MonoBehaviour
{
    [SerializeField] private List<string> unlockedSpells = new List<string>();
    [SerializeField] private List<string> equippedSpells = new List<string>();

    public List<string> GetUnlockedSpells()
    {
        return unlockedSpells;
    }

    public List<string> GetEquippedSpells()
    {
        return equippedSpells;
    }

    public void UnlockSpell(string spellId)
    {
        if (!unlockedSpells.Contains(spellId))
        {
            unlockedSpells.Add(spellId);
        }
    }

    public void EquipSpell(string spellId)
    {
        if (unlockedSpells.Contains(spellId) && !equippedSpells.Contains(spellId))
        {
            equippedSpells.Add(spellId);
        }
    }

    public void UnequipSpell(string spellId)
    {
        equippedSpells.Remove(spellId);
    }

    public void LoadUnlockedSpells(List<string> spells)
    {
        unlockedSpells.Clear();
        if (spells != null)
        {
            unlockedSpells.AddRange(spells);
        }
    }

    public void LoadEquippedSpells(List<string> spells)
    {
        equippedSpells.Clear();
        if (spells != null)
        {
            equippedSpells.AddRange(spells);
        }
    }
} 