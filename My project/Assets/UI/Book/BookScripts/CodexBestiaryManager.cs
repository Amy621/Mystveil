using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/// <summary>
/// Manages the Bestiary page in the Enchanted Codex.
/// Displays information about discovered monsters and creatures.
/// </summary>
public class CodexCreatureManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform monsterListContent;
    [SerializeField] private GameObject monsterEntryPrefab;
    
    [Header("Monster Details Panel")]
    [SerializeField] private GameObject monsterDetailsPanel;
    [SerializeField] private TextMeshProUGUI monsterNameText;
    [SerializeField] private TextMeshProUGUI monsterTypeText;
    [SerializeField] private TextMeshProUGUI monsterDescriptionText;
    [SerializeField] private TextMeshProUGUI monsterStatsText;
    [SerializeField] private TextMeshProUGUI monsterWeaknessText;
    [SerializeField] private TextMeshProUGUI monsterLocationText;
    [SerializeField] private TextMeshProUGUI monsterLootText;
    [SerializeField] private Image monsterImage;
    [SerializeField] private Button closeDetailsButton;
    
    [Header("Search")]
    [SerializeField] private TMP_InputField searchField;
    [SerializeField] private Button clearSearchButton;
    [SerializeField] private Button searchButton;
    
    [Header("Category Filters")]
    [SerializeField] private Button allCategoriesButton;
    [SerializeField] private Button beastsButton;
    [SerializeField] private Button undeadButton;
    [SerializeField] private Button elementsButton;
    [SerializeField] private Button humanoidButton;
    [SerializeField] private Button aberrationsButton;
    
    [Header("Settings")]
    [SerializeField] private Color normalEntryColor = new Color(1f, 0.9f, 0.6f);
    [SerializeField] private Color selectedEntryColor = new Color(0.6f, 1f, 0.6f);
    [SerializeField] private Color undiscoveredEntryColor = new Color(0.6f, 0.6f, 0.6f);
    [SerializeField] private Sprite silhouetteSprite;
    
    // Runtime data
    private Dictionary<string, MonsterData> discoveredMonsters = new Dictionary<string, MonsterData>();
    private List<GameObject> monsterEntryObjects = new List<GameObject>();
    private string selectedMonsterId;
    private string currentSearchText = "";
    private MonsterCategory currentFilter = MonsterCategory.All;
    private bool initialized = false;
    
    /// <summary>
    /// Initializes the Bestiary page manager.
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
        
        // Set up search functionality
        if (searchField != null)
        {
            searchField.onValueChanged.AddListener(OnSearchTextChanged);
        }
        
        if (clearSearchButton != null)
        {
            clearSearchButton.onClick.RemoveAllListeners();
            clearSearchButton.onClick.AddListener(ClearSearch);
        }
        
        if (searchButton != null)
        {
            searchButton.onClick.RemoveAllListeners();
            searchButton.onClick.AddListener(PerformSearch);
        }
        
        // Set up category filters
        SetupCategoryButton(allCategoriesButton, MonsterCategory.All);
        SetupCategoryButton(beastsButton, MonsterCategory.Beast);
        SetupCategoryButton(undeadButton, MonsterCategory.Undead);
        SetupCategoryButton(elementsButton, MonsterCategory.Elemental);
        SetupCategoryButton(humanoidButton, MonsterCategory.Humanoid);
        SetupCategoryButton(aberrationsButton, MonsterCategory.Aberration);
        
        // Hide details panel initially
        if (monsterDetailsPanel != null)
        {
            monsterDetailsPanel.SetActive(false);
        }
        
        // Populate with known monsters
        LoadKnownMonsters();
        
        initialized = true;
        Debug.Log("CodexBestiaryManager initialized");
    }
    
    private void SetupCategoryButton(Button button, MonsterCategory category)
    {
        if (button == null) return;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            currentFilter = category;
            RefreshContent();
            
            // Visual feedback for selected filter
            allCategoriesButton.interactable = (category != MonsterCategory.All);
            beastsButton.interactable = (category != MonsterCategory.Beast);
            undeadButton.interactable = (category != MonsterCategory.Undead);
            elementsButton.interactable = (category != MonsterCategory.Elemental);
            humanoidButton.interactable = (category != MonsterCategory.Humanoid);
            aberrationsButton.interactable = (category != MonsterCategory.Aberration);
        });
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
    /// Handles when search text changes.
    /// </summary>
    private void OnSearchTextChanged(string newText)
    {
        currentSearchText = newText.Trim().ToLower();
        
        // Auto-update the list if the text is empty or if it's long enough
        if (string.IsNullOrEmpty(currentSearchText) || currentSearchText.Length >= 3)
        {
            RefreshContent();
        }
    }
    
    /// <summary>
    /// Clears the search field.
    /// </summary>
    private void ClearSearch()
    {
        if (searchField != null)
        {
            searchField.text = "";
        }
        currentSearchText = "";
        RefreshContent();
    }
    
    /// <summary>
    /// Performs a search with the current search text.
    /// </summary>
    private void PerformSearch()
    {
        RefreshContent();
    }
    
    /// <summary>
    /// Discover a new monster or update an existing entry.
    /// </summary>
    public void DiscoverMonster(MonsterData monster)
    {
        if (monster == null) return;
        
        // Add or update the monster data
        discoveredMonsters[monster.monsterId] = monster;
        
        // If this page is currently visible, refresh it
        if (gameObject.activeInHierarchy)
        {
            RefreshContent();
        }
    }
    
    /// <summary>
    /// Update the encounter count for a monster.
    /// </summary>
    public void RecordMonsterEncounter(string monsterId)
    {
        if (string.IsNullOrEmpty(monsterId) || !discoveredMonsters.ContainsKey(monsterId))
            return;
            
        MonsterData monster = discoveredMonsters[monsterId];
        monster.encounterCount++;
        discoveredMonsters[monsterId] = monster;
        
        // If this monster is currently displayed, update its details
        if (selectedMonsterId == monsterId && monsterDetailsPanel != null && monsterDetailsPanel.activeSelf)
        {
            ShowMonsterDetails(monsterId);
        }
    }
    
    /// <summary>
    /// Refreshes the bestiary display.
    /// </summary>
    public void RefreshContent()
    {
        // Clear existing monster entries
        foreach (GameObject entryObj in monsterEntryObjects)
        {
            Destroy(entryObj);
        }
        monsterEntryObjects.Clear();
        
        // Filter monsters based on search text and category
        List<MonsterData> filteredMonsters = new List<MonsterData>();
        foreach (MonsterData monster in discoveredMonsters.Values)
        {
            bool matchesSearch = string.IsNullOrEmpty(currentSearchText) || 
                               monster.name.ToLower().Contains(currentSearchText) ||
                               monster.description.ToLower().Contains(currentSearchText);
                               
            bool matchesCategory = currentFilter == MonsterCategory.All || 
                                 monster.category == currentFilter;
                                 
            if (matchesSearch && matchesCategory)
            {
                filteredMonsters.Add(monster);
            }
        }
        
        // Sort monsters: first by category, then alphabetically
        filteredMonsters.Sort((a, b) => {
            int categoryComparison = a.category.CompareTo(b.category);
            if (categoryComparison != 0) return categoryComparison;
            
            return a.name.CompareTo(b.name);
        });
        
        // Create entries for each filtered monster
        foreach (MonsterData monster in filteredMonsters)
        {
            GameObject entryObj = Instantiate(monsterEntryPrefab, monsterListContent);
            monsterEntryObjects.Add(entryObj);
            
            // Set up the entry
            Button button = entryObj.GetComponent<Button>();
            TextMeshProUGUI nameText = entryObj.GetComponentInChildren<TextMeshProUGUI>();
            Image iconImage = entryObj.GetComponentsInChildren<Image>()[1]; // Assuming first is background, second is monster icon
            
            if (nameText != null)
            {
                nameText.text = monster.name;
                
                // Add kill count for completely discovered monsters
                if (monster.isFullyDiscovered)
                {
                    nameText.text += $" ({monster.encounterCount})";
                }
            }
            
            // Set icon based on discovery status
            if (iconImage != null)
            {
                iconImage.sprite = monster.isFullyDiscovered ? monster.icon : silhouetteSprite;
                iconImage.color = monster.isFullyDiscovered ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.8f);
            }
            
            // Set color based on selection status
            Image entryImage = entryObj.GetComponent<Image>();
            if (entryImage != null)
            {
                entryImage.color = (monster.monsterId == selectedMonsterId) ? 
                    selectedEntryColor : 
                    (monster.isFullyDiscovered ? normalEntryColor : undiscoveredEntryColor);
            }
            
            // Add listener to show monster details
            string id = monster.monsterId; // Capture for closure
            button.onClick.AddListener(() => ShowMonsterDetails(id));
        }
        
        // If nothing is found, maybe add a "No results" message
        if (filteredMonsters.Count == 0 && monsterEntryObjects.Count == 0)
        {
            GameObject emptyObj = Instantiate(monsterEntryPrefab, monsterListContent);
            monsterEntryObjects.Add(emptyObj);
            
            TextMeshProUGUI emptyText = emptyObj.GetComponentInChildren<TextMeshProUGUI>();
            if (emptyText != null)
            {
                emptyText.text = "No monsters discovered yet";
                if (!string.IsNullOrEmpty(currentSearchText))
                {
                    emptyText.text = "No results for \"" + currentSearchText + "\"";
                }
            }
            
            Button button = emptyObj.GetComponent<Button>();
            button.interactable = false;
            
            Image entryImage = emptyObj.GetComponent<Image>();
            if (entryImage != null)
            {
                entryImage.color = Color.clear;
            }
        }
    }
    
    private void ShowMonsterDetails(string monsterId)
    {
        if (string.IsNullOrEmpty(monsterId) || !discoveredMonsters.ContainsKey(monsterId))
            return;
            
        MonsterData monster = discoveredMonsters[monsterId];
        selectedMonsterId = monsterId;
        
        // Make sure we have necessary components
        if (monsterDetailsPanel == null || monsterNameText == null || monsterDescriptionText == null)
            return;
            
        // Show the details panel
        monsterDetailsPanel.SetActive(true);
        
        // Update UI elements
        monsterNameText.text = monster.name;
        monsterDescriptionText.text = monster.isFullyDiscovered ? monster.description : "This creature has been sighted, but details remain elusive. Defeat more of them to learn their secrets.";
        
        // Set monster type
        if (monsterTypeText != null)
        {
            monsterTypeText.text = GetCategoryName(monster.category);
        }
        
        // Build stats text
        if (monsterStatsText != null)
        {
            if (monster.isFullyDiscovered)
            {
                string statsStr = "Stats:\n";
                statsStr += $"• Health: {monster.health}\n";
                statsStr += $"• Attack: {monster.attack}\n";
                statsStr += $"• Defense: {monster.defense}\n";
                statsStr += $"• Speed: {monster.speed}\n";
                
                if (!string.IsNullOrEmpty(monster.elementType))
                {
                    statsStr += $"• Element: {monster.elementType}\n";
                }
                
                monsterStatsText.text = statsStr;
            }
            else
            {
                monsterStatsText.text = "Stats: Unknown";
            }
        }
        
        // Build weakness text
        if (monsterWeaknessText != null)
        {
            if (monster.isFullyDiscovered && monster.weaknesses != null && monster.weaknesses.Length > 0)
            {
                string weaknessStr = "Weaknesses:\n";
                foreach (string weakness in monster.weaknesses)
                {
                    weaknessStr += $"• {weakness}\n";
                }
                monsterWeaknessText.text = weaknessStr;
            }
            else
            {
                monsterWeaknessText.text = "Weaknesses: Unknown";
            }
        }
        
        // Set location info
        if (monsterLocationText != null)
        {
            monsterLocationText.text = monster.isFullyDiscovered ? 
                $"Location: {monster.location}" : 
                "Location: Unknown";
        }
        
        // Build loot text
        if (monsterLootText != null)
        {
            if (monster.isFullyDiscovered && monster.possibleLoot != null && monster.possibleLoot.Length > 0)
            {
                string lootStr = "Possible Drops:\n";
                foreach (LootItem loot in monster.possibleLoot)
                {
                    lootStr += $"• {loot.itemName} ({loot.dropChance}%)\n";
                }
                monsterLootText.text = lootStr;
            }
            else
            {
                monsterLootText.text = "Drops: Unknown";
            }
        }
        
        // Set monster image
        if (monsterImage != null)
        {
            monsterImage.sprite = monster.isFullyDiscovered ? monster.image : silhouetteSprite;
            monsterImage.color = monster.isFullyDiscovered ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.8f);
        }
        
        // Refresh the list to update selection visuals
        RefreshContent();
    }
    
    private void CloseMonsterDetails()
    {
        if (monsterDetailsPanel != null)
        {
            monsterDetailsPanel.SetActive(false);
        }
        selectedMonsterId = null;
        
        // Refresh the list to update selection visuals
        RefreshContent();
    }
    
    /// <summary>
    /// Gets a friendly name for a monster category.
    /// </summary>
    private string GetCategoryName(MonsterCategory category)
    {
        switch (category)
        {
            case MonsterCategory.Beast: return "Beast";
            case MonsterCategory.Undead: return "Undead";
            case MonsterCategory.Elemental: return "Elemental";
            case MonsterCategory.Humanoid: return "Humanoid";
            case MonsterCategory.Aberration: return "Aberration";
            default: return "Unknown";
        }
    }
    
    /// <summary>
    /// Loads all known monsters from the database.
    /// </summary>
    private void LoadKnownMonsters()
    {
        // This is where you would typically load from a scriptable object database or similar
        // For now, we'll just add a few test monsters
        
        // This would be replaced by your actual data loading logic
        Dictionary<string, MonsterData> allMonsters = MonsterCatalog.GetAllMonsters();
        foreach (MonsterData monster in allMonsters.Values)
        {
            // Only add if it's been discovered
            if (monster.isDiscovered)
            {
                discoveredMonsters[monster.monsterId] = monster;
            }
        }
    }
    
    /// <summary>
    /// Handles saving bestiary data.
    /// </summary>
    public void OnSave(SimpleSaveData saveData)
    {
        // Save discovered monster IDs
        string[] monsterIds = new string[discoveredMonsters.Count];
        int index = 0;
        
        foreach (KeyValuePair<string, MonsterData> entry in discoveredMonsters)
        {
            monsterIds[index] = entry.Key;
            
            // Save individual monster data
            saveData.SetBool($"monster_{entry.Key}_discovered", true);
            saveData.SetBool($"monster_{entry.Key}_fully_discovered", entry.Value.isFullyDiscovered);
            saveData.SetInt($"monster_{entry.Key}_encounter_count", entry.Value.encounterCount);
            
            index++;
        }
        
        // Save the list of all discovered monster IDs
        saveData.SetStringArray("discovered_monsters", monsterIds);
        
        Debug.Log($"CodexBestiaryManager: Saved {discoveredMonsters.Count} monster entries");
    }
    
    /// <summary>
    /// Handles loading bestiary data.
    /// </summary>
    public void OnLoad(SimpleSaveData saveData)
    {
        // Clear current data
        discoveredMonsters.Clear();
        
        // Get all monster IDs
        string[] monsterIds = saveData.GetStringArray("discovered_monsters");
        
        if (monsterIds != null && monsterIds.Length > 0)
        {
            // Get reference to all monsters
            Dictionary<string, MonsterData> allMonsters = MonsterCatalog.GetAllMonsters();
            
            // Load each monster
            foreach (string id in monsterIds)
            {
                // Skip if monster doesn't exist in the database
                if (!allMonsters.ContainsKey(id)) continue;
                
                // Get a copy of the monster data
                MonsterData monster = allMonsters[id];
                
                // Apply saved properties
                monster.isDiscovered = saveData.GetBool($"monster_{id}_discovered", true);
                monster.isFullyDiscovered = saveData.GetBool($"monster_{id}_fully_discovered", false);
                monster.encounterCount = saveData.GetInt($"monster_{id}_encounter_count", 0);
                
                // Add to discovered monsters
                discoveredMonsters[id] = monster;
            }
            
            Debug.Log($"CodexBestiaryManager: Loaded {discoveredMonsters.Count} monster entries");
        }
        else
        {
            Debug.Log("CodexBestiaryManager: No monster data found");
        }
        
        // Refresh UI
        RefreshContent();
    }
    
    /// <summary>
    /// Monster category enum
    /// </summary>
    public enum MonsterCategory
    {
        All,
        Beast,
        Undead,
        Elemental,
        Humanoid,
        Aberration
    }
    
    /// <summary>
    /// Data class for monsters.
    /// </summary>
    [System.Serializable]
    public class MonsterData
    {
        public string monsterId;
        public string name;
        public string description;
        public MonsterCategory category;
        public string location;
        public int health;
        public int attack;
        public int defense;
        public int speed;
        public string elementType;
        public string[] weaknesses;
        public LootItem[] possibleLoot;
        public Sprite icon;
        public Sprite image;
        
        // Runtime/saved properties
        public bool isDiscovered;
        public bool isFullyDiscovered;
        public int encounterCount;
    }
    
    /// <summary>
    /// Data class for monster loot items.
    /// </summary>
    [System.Serializable]
    public class LootItem
    {
        public string itemId;
        public string itemName;
        public float dropChance; // percentage, e.g. 25.5 means 25.5%
    }
    
    /// <summary>
    /// Static utility class for accessing monster data.
    /// </summary>
    public static class MonsterCatalog
    {
        private static Dictionary<string, MonsterData> allMonsters;
        
        /// <summary>
        /// Gets all monsters in the game.
        /// </summary>
        public static Dictionary<string, MonsterData> GetAllMonsters()
        {
            if (allMonsters == null)
            {
                // In a real implementation, this would load from a database or scriptable objects
                InitializeMonsterDatabase();
            }
            
            return allMonsters;
        }
        
        /// <summary>
        /// Gets a specific monster by ID.
        /// </summary>
        public static MonsterData GetMonsterById(string monsterId)
        {
            if (allMonsters == null)
            {
                InitializeMonsterDatabase();
            }
            
            if (allMonsters.ContainsKey(monsterId))
            {
                return allMonsters[monsterId];
            }
            
            return null;
        }
        
        private static void InitializeMonsterDatabase()
        {
            allMonsters = new Dictionary<string, MonsterData>();
            
            // In a real implementation, you would load these from ScriptableObjects or a database
            // This is just a placeholder example
            
            // Load from Resources folder instead
            ScriptableMonster[] monsterScriptables = Resources.LoadAll<ScriptableMonster>("Monsters");
            
            foreach (ScriptableMonster monster in monsterScriptables)
            {
                CodexMonstersManager.MonsterData sourceData = monster.GetMonsterData();
                
                // Convert from CodexMonstersManager.MonsterData to our MonsterData
                MonsterData convertedData = new MonsterData
                {
                    monsterId = sourceData.monsterId,
                    name = sourceData.monsterName,
                    description = sourceData.description,
                    category = MonsterCategory.Beast, // Default or map from monsterType
                    location = sourceData.locations.Length > 0 ? sourceData.locations[0] : "Unknown",
                    health = sourceData.health,
                    attack = sourceData.attack,
                    defense = sourceData.defense,
                    speed = 0, // Not available in source
                    elementType = "",
                    weaknesses = new string[0],
                    possibleLoot = sourceData.possibleDrops?.Select(d => new LootItem { 
                        itemId = d.itemId, 
                        itemName = d.itemName, 
                        dropChance = d.dropChance 
                    }).ToArray() ?? new LootItem[0],
                    icon = sourceData.portraitSprite,
                    image = sourceData.portraitSprite,
                    isDiscovered = true,
                    isFullyDiscovered = true,
                    encounterCount = 0
                };
                
                allMonsters[convertedData.monsterId] = convertedData;
            }
            
            Debug.Log($"MonsterCatalog: Loaded {allMonsters.Count} monsters");
        }
    }
} 