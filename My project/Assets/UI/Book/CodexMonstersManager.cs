using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

/// <summary>
/// Manages the Monsters page in the Enchanted Codex.
/// Displays discovered creatures with their stats, descriptions, and locations.
/// </summary>
public class CodexMonstersManager : MonoBehaviour
{
    // Static reference to the monster catalog
    public static MonsterCatalog MonsterCatalog => MonsterCatalog.Instance;
    
    [Header("UI References")]
    [SerializeField] private Transform monstersContent;
    [SerializeField] private GameObject monsterEntryPrefab;
    
    [Header("Monster Details Panel")]
    [SerializeField] private GameObject monsterDetailsPanel;
    [SerializeField] private TextMeshProUGUI monsterNameText;
    [SerializeField] private TextMeshProUGUI monsterTypeText;
    [SerializeField] private TextMeshProUGUI monsterDescriptionText;
    [SerializeField] private TextMeshProUGUI monsterStatsText;
    [SerializeField] private TextMeshProUGUI monsterDropsText;
    [SerializeField] private TextMeshProUGUI monsterLocationsText;
    [SerializeField] private Image monsterPortraitImage;
    [SerializeField] private Button closeDetailsButton;
    
    [Header("Settings")]
    [SerializeField] private Color commonMonsterColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color uncommonMonsterColor = new Color(0.6f, 0.9f, 0.6f);
    [SerializeField] private Color rareMonsterColor = new Color(0.6f, 0.8f, 1.0f);
    [SerializeField] private Color epicMonsterColor = new Color(0.8f, 0.6f, 1.0f);
    [SerializeField] private Color legendaryMonsterColor = new Color(1.0f, 0.8f, 0.4f);
    [SerializeField] private Color bossMonsterColor = new Color(1.0f, 0.5f, 0.5f);
    
    // Runtime data
    private Dictionary<string, MonsterData> discoveredMonsters = new Dictionary<string, MonsterData>();
    private List<GameObject> monsterButtons = new List<GameObject>();
    private string selectedMonsterId;
    private bool initialized = false;
    
    /// <summary>
    /// Initializes the Monsters page manager.
    /// </summary>
    public void Initialize()
    {
        if (initialized) return;
        
        // Set up close button
        if (closeDetailsButton != null)
        {
            closeDetailsButton.onClick.RemoveAllListeners();
            closeDetailsButton.onClick.AddListener(CloseMonsterDetails);
        }
        
        // Hide details panel initially
        if (monsterDetailsPanel != null)
        {
            monsterDetailsPanel.SetActive(false);
        }
        
        // Load monsters from Resources
        LoadMonsters();
        
        initialized = true;
        Debug.Log("CodexMonstersManager initialized");
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
    }
    
    private void OnEnable()
    {
        // Refresh content when shown
        RefreshContent();
    }
    
    private void OnDisable()
    {
        // Close details panel if showing
        if (monsterDetailsPanel != null && monsterDetailsPanel.activeSelf)
        {
            monsterDetailsPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Loads monsters from Resources folder.
    /// </summary>
    private void LoadMonsters()
    {
        ScriptableMonster[] monsters = Resources.LoadAll<ScriptableMonster>("1V1/Monsters");
        
        if (monsters != null && monsters.Length > 0)
        {
            foreach (var monster in monsters)
            {
                // In a real game, you'd check if the monster is discovered
                // For now, just add all monsters as discovered
                MonsterData monsterData = monster.GetMonsterData();
                discoveredMonsters[monsterData.monsterId] = monsterData;
            }
            
            Debug.Log($"Loaded {discoveredMonsters.Count} monsters from Resources");
        }
        else
        {
            Debug.LogWarning("No monsters found in Resources/1V1/Monsters");
        }
    }
    
    /// <summary>
    /// Event handler for when a new monster is discovered.
    /// </summary>
    public void OnMonsterDiscovered(string monsterId)
    {
        // Add the monster to our discovered list if not already there
        if (!discoveredMonsters.ContainsKey(monsterId))
        {
            // For this example, we're assuming there's a method to get monster data by ID
            ScriptableMonster monster = Resources.Load<ScriptableMonster>("1V1/Monsters/" + monsterId);
            if (monster != null)
            {
                MonsterData monsterData = monster.GetMonsterData();
                discoveredMonsters[monsterId] = monsterData;
                
                // If this page is currently visible, refresh it
                if (gameObject.activeInHierarchy)
                {
                    RefreshContent();
                }
            }
        }
    }
    
    /// <summary>
    /// Refreshes the monsters display.
    /// </summary>
    public void RefreshContent()
    {
        // Clear existing monster buttons
        foreach (GameObject buttonObj in monsterButtons)
        {
            Destroy(buttonObj);
        }
        monsterButtons.Clear();
        
        // Sort monsters by type, then rarity, then name
        List<MonsterData> sortedMonsters = new List<MonsterData>(discoveredMonsters.Values);
        sortedMonsters.Sort((a, b) => {
            // First by type
            int typeComparison = a.monsterType.CompareTo(b.monsterType);
            if (typeComparison != 0) return typeComparison;
            
            // Then by rarity (assuming higher number = more rare)
            int rarityComparison = b.rarityLevel.CompareTo(a.rarityLevel);
            if (rarityComparison != 0) return rarityComparison;
            
            // Finally by name
            return a.monsterName.CompareTo(b.monsterName);
        });
        
        // Create a button for each discovered monster
        foreach (MonsterData monster in sortedMonsters)
        {
            GameObject buttonObj = Instantiate(monsterEntryPrefab, monstersContent);
            monsterButtons.Add(buttonObj);
            
            // Set up the button
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI nameText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            Image iconImage = buttonObj.transform.Find("MonsterIcon")?.GetComponent<Image>();
            
            if (nameText != null)
            {
                nameText.text = monster.monsterName;
            }
            
            if (iconImage != null && monster.portraitSprite != null)
            {
                iconImage.sprite = monster.portraitSprite;
            }
            
            // Apply color based on rarity
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = GetMonsterRarityColor(monster.rarityLevel, monster.isBoss);
            }
            
            // Add listener to show monster details
            string id = monster.monsterId; // Capture for closure
            button.onClick.AddListener(() => ShowMonsterDetails(id));
        }
    }
    
    private Color GetMonsterRarityColor(int rarityLevel, bool isBoss)
    {
        if (isBoss)
            return bossMonsterColor;
            
        switch (rarityLevel)
        {
            case 1: return commonMonsterColor;
            case 2: return uncommonMonsterColor;
            case 3: return rareMonsterColor;
            case 4: return epicMonsterColor;
            case 5: return legendaryMonsterColor;
            default: return commonMonsterColor;
        }
    }
    
    private string GetRarityName(int rarityLevel)
    {
        switch (rarityLevel)
        {
            case 1: return "Common";
            case 2: return "Uncommon";
            case 3: return "Rare";
            case 4: return "Epic";
            case 5: return "Legendary";
            default: return "Unknown";
        }
    }
    
    private void ShowMonsterDetails(string monsterId)
    {
        if (!discoveredMonsters.TryGetValue(monsterId, out MonsterData monster))
            return;
            
        selectedMonsterId = monsterId;
        
        // Make sure we have necessary components
        if (monsterDetailsPanel == null || monsterNameText == null)
            return;
            
        // Show the details panel
        monsterDetailsPanel.SetActive(true);
        
        // Update UI elements
        monsterNameText.text = monster.monsterName;
        
        if (monsterTypeText != null)
        {
            string typeText = monster.monsterType;
            if (monster.isBoss)
                typeText += " (Boss)";
            else
                typeText += $" ({GetRarityName(monster.rarityLevel)})";
                
            monsterTypeText.text = typeText;
        }
        
        if (monsterDescriptionText != null)
            monsterDescriptionText.text = monster.description;
        
        if (monsterStatsText != null)
        {
            string statsStr = "Stats:\n";
            statsStr += $"• Level: {monster.level}\n";
            statsStr += $"• Health: {monster.health}\n";
            statsStr += $"• Attack: {monster.attack}\n";
            statsStr += $"• Defense: {monster.defense}\n";
            statsStr += $"• Magic Resist: {monster.magicResistance}\n";
            
            if (monster.specialAbilities != null && monster.specialAbilities.Length > 0)
            {
                statsStr += "Special Abilities:\n";
                foreach (string ability in monster.specialAbilities)
                {
                    statsStr += $"• {ability}\n";
                }
            }
            
            monsterStatsText.text = statsStr;
        }
        
        if (monsterDropsText != null && monster.possibleDrops != null)
        {
            string dropsStr = "Possible Drops:\n";
            
            if (monster.possibleDrops.Length > 0)
            {
                foreach (MonsterDrop drop in monster.possibleDrops)
                {
                    dropsStr += $"• {drop.itemName} ({drop.dropChance}%)\n";
                }
            }
            else
            {
                dropsStr += "None";
            }
            
            monsterDropsText.text = dropsStr;
        }
        
        if (monsterLocationsText != null && monster.locations != null)
        {
            string locationsStr = "Found in:\n";
            
            if (monster.locations.Length > 0)
            {
                foreach (string location in monster.locations)
                {
                    locationsStr += $"• {location}\n";
                }
            }
            else
            {
                locationsStr += "Unknown";
            }
            
            monsterLocationsText.text = locationsStr;
        }
        
        if (monsterPortraitImage != null && monster.portraitSprite != null)
            monsterPortraitImage.sprite = monster.portraitSprite;
    }
    
    private void CloseMonsterDetails()
    {
        if (monsterDetailsPanel != null)
        {
            monsterDetailsPanel.SetActive(false);
        }
        selectedMonsterId = null;
    }
    
    public void OnSave(SimpleSaveData saveData)
    {
        // Save discovered monster IDs
        saveData.SetStringArray("discovered_monsters", discoveredMonsters.Keys.ToArray());
    }
    
    public void OnLoad(SimpleSaveData saveData)
    {
        // Clear current data
        discoveredMonsters.Clear();
        
        // Load all monsters first
        Dictionary<string, MonsterData> allMonsters = new Dictionary<string, MonsterData>();
        ScriptableMonster[] monsters = Resources.LoadAll<ScriptableMonster>("1V1/Monsters");
        foreach (var monster in monsters)
        {
            MonsterData monsterData = monster.GetMonsterData();
            allMonsters[monsterData.monsterId] = monsterData;
        }
        
        // Get discovered monster IDs
        string[] discoveredIds = saveData.GetStringArray("discovered_monsters");
        if (discoveredIds != null && discoveredIds.Length > 0)
        {
            foreach (string id in discoveredIds)
            {
                if (allMonsters.ContainsKey(id))
                {
                    discoveredMonsters[id] = allMonsters[id];
                }
            }
            
            Debug.Log($"Loaded {discoveredMonsters.Count} discovered monsters");
        }
        else
        {
            // For testing, consider all monsters discovered
            discoveredMonsters = new Dictionary<string, MonsterData>(allMonsters);
            Debug.Log($"No saved monster discoveries, loaded all {discoveredMonsters.Count} monsters for testing");
        }
        
        // Refresh UI
        RefreshContent();
    }
    
    [System.Serializable]
    public class MonsterData
    {
        public string monsterId;
        public string monsterName;
        public string monsterType;  // Undead, Beast, Humanoid, etc.
        public string description;
        public int rarityLevel;     // 1-5, with 5 being most rare
        public int level;
        public int health;
        public int attack;
        public int defense;
        public int magicResistance;
        public bool isBoss;
        public string[] specialAbilities;
        public string[] locations;
        public MonsterDrop[] possibleDrops;
        public Sprite portraitSprite;
    }
    
    [System.Serializable]
    public class MonsterDrop
    {
        public string itemId;
        public string itemName;
        public float dropChance;  // As a percentage (0-100)
    }
} 