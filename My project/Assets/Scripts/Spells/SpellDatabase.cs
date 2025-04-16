using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpellDatabase", menuName = "Game/Databases/Spell Database")]
public class SpellDatabase : ScriptableObject
{
    [SerializeField] private List<Spell> spells = new List<Spell>();
    
    // Get spell by ID
    public Spell GetSpellByID(string spellID)
    {
        return spells.Find(spell => spell.SpellID == spellID);
    }
    
    // Get all spells
    public List<Spell> GetAllSpells()
    {
        return spells;
    }
}

// Placeholder Spell class
[System.Serializable]
public class Spell
{
    [SerializeField] private string spellID;
    [SerializeField] private string spellName;
    [SerializeField] private string description;
    [SerializeField] private float manaCost;
    [SerializeField] private float cooldown;
    
    // Properties
    public string SpellID => spellID;
    public string SpellName => spellName;
    public string Description => description;
    public float ManaCost => manaCost;
    public float Cooldown => cooldown;
} 