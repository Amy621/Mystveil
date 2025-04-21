using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject database for all monsters in the game.
/// This acts as the central registry of monster data.
/// </summary>
[CreateAssetMenu(fileName = "MonsterDatabase", menuName = "Game Data/Monster Database")]
public class MonsterDatabase : ScriptableObject
{
    [SerializeField] private List<MonsterEntry> monsters = new List<MonsterEntry>();
    
    // Dictionary for quick access by ID
    private Dictionary<string, MonsterEntry> monsterDict;
    
    // Initialize the dictionary on first access
    private void InitializeDictionary()
    {
        if (monsterDict == null)
        {
            monsterDict = new Dictionary<string, MonsterEntry>();
            foreach (var monster in monsters)
            {
                if (!string.IsNullOrEmpty(monster.id))
                {
                    monsterDict[monster.id] = monster;
                }
                else
                {
                    Debug.LogWarning("Monster with empty ID found in database", this);
                }
            }
        }
    }
    
    /// <summary>
    /// Gets a list of all monsters in the database.
    /// </summary>
    public List<MonsterEntry> GetAllMonsters()
    {
        return monsters;
    }
    
    /// <summary>
    /// Gets a monster by its unique ID.
    /// </summary>
    public MonsterEntry GetMonsterById(string id)
    {
        InitializeDictionary();
        
        if (monsterDict.TryGetValue(id, out MonsterEntry monster))
        {
            return monster;
        }
        
        return null;
    }
    
    /// <summary>
    /// Gets monsters of a specific type.
    /// </summary>
    public List<MonsterEntry> GetMonstersByType(string type)
    {
        List<MonsterEntry> result = new List<MonsterEntry>();
        
        foreach (var monster in monsters)
        {
            if (monster.type == type)
            {
                result.Add(monster);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Converts all monsters to the format used by the CodexMonstersManager.
    /// </summary>
    public List<CodexMonstersManager.MonsterData> GetCodexMonsterData()
    {
        List<CodexMonstersManager.MonsterData> result = new List<CodexMonstersManager.MonsterData>();
        
        foreach (var monster in monsters)
        {
            CodexMonstersManager.MonsterData codexData = new CodexMonstersManager.MonsterData
            {
                monsterId = monster.id,
                monsterName = monster.displayName,
                monsterType = monster.type,
                description = monster.description,
                rarityLevel = monster.rarityLevel,
                level = monster.level,
                health = monster.health,
                attack = monster.attack,
                defense = monster.defense,
                magicResistance = monster.magicResistance,
                isBoss = monster.isBoss,
                specialAbilities = monster.specialAbilities.ToArray(),
                locations = monster.locations.ToArray(),
                portraitSprite = monster.portrait
            };
            
            // Convert item drops
            List<CodexMonstersManager.MonsterDrop> drops = new List<CodexMonstersManager.MonsterDrop>();
            foreach (var drop in monster.possibleDrops)
            {
                drops.Add(new CodexMonstersManager.MonsterDrop
                {
                    itemId = drop.itemId,
                    itemName = drop.itemName,
                    dropChance = drop.dropChance
                });
            }
            codexData.possibleDrops = drops.ToArray();
            
            result.Add(codexData);
        }
        
        return result;
    }
}

/// <summary>
/// Data class for a single monster entry in the database.
/// </summary>
[System.Serializable]
public class MonsterEntry
{
    [Header("Basic Info")]
    public string id;
    public string displayName;
    public string type;  // Undead, Beast, Humanoid, etc.
    [TextArea(3, 5)]
    public string description;
    [Range(1, 5)]
    public int rarityLevel = 1;  // 1-5, with 5 being most rare
    public bool isBoss = false;
    
    [Header("Stats")]
    public int level = 1;
    public int health = 10;
    public int attack = 1;
    public int defense = 0;
    public int magicResistance = 0;
    
    [Header("Visual")]
    public Sprite portrait;
    
    [Header("Additional Info")]
    public List<string> specialAbilities = new List<string>();
    public List<string> locations = new List<string>();
    public List<ItemDrop> possibleDrops = new List<ItemDrop>();
    
    [System.Serializable]
    public class ItemDrop
    {
        public string itemId;
        public string itemName;
        [Range(0, 100)]
        public float dropChance = 10f;  // As a percentage (0-100)
    }
} 