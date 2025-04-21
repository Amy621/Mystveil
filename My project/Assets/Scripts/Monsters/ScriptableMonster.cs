using UnityEngine;
using System.Linq;
using static CodexMonstersManager;

/// <summary>
/// Scriptable object that defines a monster's data for the game.
/// </summary>
[CreateAssetMenu(fileName = "New Monster", menuName = "Mystveil/Monster")]
public class ScriptableMonster : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string monsterId;
    [SerializeField] private string monsterName;
    [SerializeField] [TextArea(3, 5)] private string description;
    
    [Header("Category")]
    [SerializeField] private string monsterType;
    [SerializeField] private string[] locations;
    [SerializeField] private int rarityLevel = 1;
    [SerializeField] private bool isBoss;
    
    [Header("Stats")]
    [SerializeField] private int level = 1;
    [SerializeField] private int health = 100;
    [SerializeField] private int attack = 10;
    [SerializeField] private int defense = 5;
    [SerializeField] private int magicResistance = 0;
    [SerializeField] private string[] specialAbilities;
    
    [Header("Appearance")]
    [SerializeField] private Sprite portraitSprite;
    
    [Header("Loot")]
    [SerializeField] private LootDefinition[] possibleLoot;
    
    /// <summary>
    /// Generates a MonsterData object for use in the Bestiary.
    /// </summary>
    public CodexMonstersManager.MonsterData GetMonsterData()
    {
        // Generate a default ID if not explicitly set
        if (string.IsNullOrEmpty(monsterId))
        {
            monsterId = name.ToLower().Replace(" ", "_");
        }
        
        // Convert the loot definitions to runtime loot items
        CodexMonstersManager.MonsterDrop[] lootItems = null;
        if (possibleLoot != null && possibleLoot.Length > 0)
        {
            lootItems = possibleLoot.Select(l => new CodexMonstersManager.MonsterDrop
            {
                itemId = l.itemId,
                itemName = l.itemName,
                dropChance = l.dropChance
            }).ToArray();
        }
        
        return new CodexMonstersManager.MonsterData
        {
            monsterId = monsterId,
            monsterName = string.IsNullOrEmpty(monsterName) ? name : monsterName,
            monsterType = monsterType,
            description = description,
            rarityLevel = rarityLevel,
            level = level,
            health = health,
            attack = attack,
            defense = defense,
            magicResistance = magicResistance,
            isBoss = isBoss,
            specialAbilities = specialAbilities ?? new string[0],
            locations = locations ?? new string[0],
            portraitSprite = portraitSprite,
            possibleDrops = lootItems ?? new CodexMonstersManager.MonsterDrop[0]
        };
    }
    
    /// <summary>
    /// Serializable class for defining loot in the inspector.
    /// </summary>
    [System.Serializable]
    public class LootDefinition
    {
        public string itemId;
        public string itemName;
        [Range(0f, 100f)]
        public float dropChance = 25f;
    }
} 