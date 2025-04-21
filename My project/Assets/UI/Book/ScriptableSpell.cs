using UnityEngine;

/// <summary>
/// Scriptable object that defines a spell's properties.
/// </summary>
[CreateAssetMenu(fileName = "New Spell", menuName = "Mystveil/Spell")]
public class ScriptableSpell : ScriptableObject
{
    [Header("Basic Info")]
    public new string name;
    public SpellType spellType;
    [TextArea(3, 6)]
    public string description;
    public Sprite icon;
    
    [Header("Spell Stats")]
    public int manaCost = 10;
    public float damage = 5f;
    public float cooldown = 2f;
    public float range = 10f;
    public float areaOfEffect = 0f;
    public float castTime = 0.5f;
    
    [Header("Effects")]
    public bool applyStatusEffect = false;
    public string statusEffectName;
    public float statusEffectDuration = 0f;
    
    [Header("Sound & VFX")]
    public AudioClip castSound;
    public GameObject castEffect;
    public GameObject impactEffect;
}

/// <summary>
/// Enum representing different spell types.
/// </summary>
public enum SpellType
{
    Fire,
    Ice,
    Lightning,
    Earth,
    Wind,
    Water,
    Light,
    Dark,
    Arcane,
    Nature,
    Healing,
    Buff,
    Debuff,
    Utility
} 