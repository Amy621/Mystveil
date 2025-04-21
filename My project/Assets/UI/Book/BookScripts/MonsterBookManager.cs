using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class MonsterBookManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform monsterGridContainer;
    [SerializeField] private GameObject monsterCardPrefab;
    [SerializeField] private GameObject monsterDetailPanel;
    
    [Header("Detail View References")]
    [SerializeField] private Image detailImage;
    [SerializeField] private TextMeshProUGUI monsterNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statsText;
    
    [Header("Monster Data")]
    [SerializeField] private EnemyStats[] monsterScriptables;
    
    [Header("References (Optional)")]
    [Tooltip("Optional reference to grid generator to auto-populate monster data")]
    [SerializeField] private GenerateGrid gridGenerator;
    
    private Dictionary<string, EnemyStats> monsterDictionary = new Dictionary<string, EnemyStats>();
    private Dictionary<string, bool> monsterDiscoveryStatus = new Dictionary<string, bool>();
    
    private void Awake()
    {
        Debug.Log("MonsterBookManager Awake - Initializing");
        
        // If grid generator is assigned, try to get monster scriptable objects from it
        if (gridGenerator != null && monsterScriptables.Length == 0)
        {
            AutoPopulateFromGridGenerator();
        }
        
        // Initialize dictionaries
        foreach (var monster in monsterScriptables)
        {
            if (monster != null)
            {
                string monsterId = monster.name;
                monsterDictionary[monsterId] = monster;
                monsterDiscoveryStatus[monsterId] = false; // Default to undiscovered
                Debug.Log($"Added monster to dictionary: {monsterId}");
            }
        }
        
        // Load discovery status from save system
        LoadDiscoveryStatus();
        
        // Initially populate grid
        PopulateMonsterGrid();
        
        // Ensure detail panel is set up
        if (monsterDetailPanel != null)
        {
            monsterDetailPanel.SetActive(false); // Make sure it's initially hidden
        }
        else
        {
            Debug.LogError("Monster Detail Panel is not assigned!");
        }
    }

    private void AutoPopulateFromGridGenerator()
    {
        Debug.Log("Auto-populating monster list from grid generator");
        
        // Try to get monsters from grid generator's public fields
        List<EnemyStats> stats = new List<EnemyStats>();
        
        if (gridGenerator.GuardianOfTheWellObject != null) stats.Add(gridGenerator.GuardianOfTheWellObject);
        if (gridGenerator.LunarFenrirObject != null) stats.Add(gridGenerator.LunarFenrirObject);
        if (gridGenerator.RootboundTyrantObject != null) stats.Add(gridGenerator.RootboundTyrantObject);
        if (gridGenerator.CottonTailObject != null) stats.Add(gridGenerator.CottonTailObject);
        if (gridGenerator.WebweaverObject != null) stats.Add(gridGenerator.WebweaverObject);
        if (gridGenerator.AsterEyeObject != null) stats.Add(gridGenerator.AsterEyeObject);
        if (gridGenerator.DionaeantObject != null) stats.Add(gridGenerator.DionaeantObject);
        if (gridGenerator.StranterryObject != null) stats.Add(gridGenerator.StranterryObject);
        if (gridGenerator.BantbooObject != null) stats.Add(gridGenerator.BantbooObject);
        if (gridGenerator.PollantObject != null) stats.Add(gridGenerator.PollantObject);
        if (gridGenerator.BriarheartObject != null) stats.Add(gridGenerator.BriarheartObject);
        if (gridGenerator.OdosaplingObject != null) stats.Add(gridGenerator.OdosaplingObject);
        if (gridGenerator.VilebloomObject != null) stats.Add(gridGenerator.VilebloomObject);
        
        // Apply to our array
        monsterScriptables = stats.ToArray();
        Debug.Log($"Auto-populated {monsterScriptables.Length} monsters from grid generator");
    }

    private void OnEnable()
    {
        // Subscribe to save/load events
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.OnSave += SaveDiscoveryStatus;
            SimpleSaveSystem.Instance.OnLoad += LoadDiscoveryStatus;
            Debug.Log("[MonsterBookManager] Successfully subscribed to SimpleSaveSystem events");
        }
        else
        {
            Debug.LogWarning("[MonsterBookManager] SimpleSaveSystem.Instance is null! Save/load functionality won't work.");
            
            // Try to find the SimpleSaveSystem in the scene if it exists
            SimpleSaveSystem saveSystem = FindObjectOfType<SimpleSaveSystem>();
            if (saveSystem != null)
            {
                Debug.Log("[MonsterBookManager] Found SimpleSaveSystem in scene, subscribing to events");
                saveSystem.OnSave += SaveDiscoveryStatus;
                saveSystem.OnLoad += LoadDiscoveryStatus;
            }
            else
            {
                Debug.LogError("[MonsterBookManager] No SimpleSaveSystem found in the scene!");
            }
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from save/load events
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.OnSave -= SaveDiscoveryStatus;
            SimpleSaveSystem.Instance.OnLoad -= LoadDiscoveryStatus;
            Debug.Log("[MonsterBookManager] Unsubscribed from SimpleSaveSystem events");
        }
        
        // Safety check - find any SimpleSaveSystem in the scene
        SimpleSaveSystem saveSystem = FindObjectOfType<SimpleSaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.OnSave -= SaveDiscoveryStatus;
            saveSystem.OnLoad -= LoadDiscoveryStatus;
        }
    }
    
    private void SaveDiscoveryStatus(SimpleSaveData saveData)
    {
        // Save the list of discovered monster IDs
        List<string> discoveredMonsters = new List<string>();
        
        foreach (var entry in monsterDiscoveryStatus)
        {
            if (entry.Value) // If monster is discovered
            {
                discoveredMonsters.Add(entry.Key);
            }
        }
        
        // Save to the save data under a specific key
        saveData.SetStringList("monster_book_discovered", discoveredMonsters);
        
        // Also save as individual flags for better compatibility
        foreach (var entry in monsterDiscoveryStatus)
        {
            string key = "monster_discovered_" + entry.Key;
            saveData.SetBool(key, entry.Value);
        }
        
        Debug.Log($"[MonsterBookManager] Saved {discoveredMonsters.Count} discovered monsters");
        if (discoveredMonsters.Count > 0)
        {
            Debug.Log($"[MonsterBookManager] First few monsters: {string.Join(", ", discoveredMonsters.Take(3))}");
        }
    }
    
    private void LoadDiscoveryStatus()
    {
        // Reset all to undiscovered by default
        foreach (var monsterId in monsterDictionary.Keys.ToList())
        {
            monsterDiscoveryStatus[monsterId] = false;
        }
        
        // Try to load from save system if available
        if (SimpleSaveSystem.Instance != null)
        {
            // Check if a save file exists
            string savePath = SimpleSaveSystem.Instance.SaveFilePath;
            if (System.IO.File.Exists(savePath))
            {
                // Save file exists, but we'll let the SimpleSaveSystem handle loading it
                // Our OnLoad event will be called when it does
                return;
            }
        }
        
        // If no save system or no save file, just use default values (all undiscovered)
        PopulateMonsterGrid();
    }
    
    private void LoadDiscoveryStatus(SimpleSaveData saveData)
    {
        // Reset all to undiscovered
        foreach (var monsterId in monsterDictionary.Keys.ToList())
        {
            monsterDiscoveryStatus[monsterId] = false;
        }
        
        bool loadedAny = false;
        
        // First try to load from the list
        List<string> discoveredMonsters = saveData.GetStringList("monster_book_discovered");
        
        if (discoveredMonsters != null && discoveredMonsters.Count > 0)
        {
            loadedAny = true;
            foreach (var monsterId in discoveredMonsters)
            {
                if (monsterDictionary.ContainsKey(monsterId))
                {
                    monsterDiscoveryStatus[monsterId] = true;
                    Debug.Log($"[MonsterBookManager] Monster {monsterId} is marked as discovered (from list)");
                }
            }
        }
        
        // As a backup, also check individual flags
        if (!loadedAny)
        {
            foreach (var monsterId in monsterDictionary.Keys)
            {
                string key = "monster_discovered_" + monsterId;
                if (saveData.GetBool(key, false))
                {
                    monsterDiscoveryStatus[monsterId] = true;
                    Debug.Log($"[MonsterBookManager] Monster {monsterId} is marked as discovered (from individual flag)");
                    loadedAny = true;
                }
            }
        }
        
        // Log a message if nothing was loaded
        if (!loadedAny)
        {
            Debug.Log("[MonsterBookManager] No discovered monsters found in save data");
        }
        
        // Refresh the grid with updated discovery status
        PopulateMonsterGrid();
    }
    
    private void PopulateMonsterGrid()
    {
        Debug.Log("Populating monster grid");
        // Clear existing cards
        foreach (Transform child in monsterGridContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create cards for each monster
        foreach (var entry in monsterDictionary)
        {
            string monsterId = entry.Key;
            EnemyStats monster = entry.Value;
            
            if (monster != null)
            {
                GameObject card = Instantiate(monsterCardPrefab, monsterGridContainer);
                SetupMonsterCard(card, monster, monsterDiscoveryStatus[monsterId]);
                Debug.Log($"Created card for monster: {monsterId} (Discovered: {monsterDiscoveryStatus[monsterId]})");
            }
        }
    }
    
    private void SetupMonsterCard(GameObject card, EnemyStats monster, bool isDiscovered)
    {
        // Get components
        Image iconImage = card.GetComponentInChildren<Image>();
        TextMeshProUGUI nameText = card.GetComponentInChildren<TextMeshProUGUI>();
        Button cardButton = card.GetComponent<Button>();
        
        if (iconImage == null) Debug.LogError($"Icon Image component missing on card for {monster.name}");
        if (nameText == null) Debug.LogError($"TextMeshPro component missing on card for {monster.name}");
        if (cardButton == null) Debug.LogError($"Button component missing on card for {monster.name}");
        
        // Set data
        if (isDiscovered)
        {
            Debug.Log($"Setting up discovered monster card: {monster.name}");
            
            if (monster.MonsterIcon != null)
            {
                iconImage.sprite = monster.MonsterIcon;
            }
            else
            {
                Debug.LogError($"Monster icon is missing for {monster.name}");
            }
            nameText.text = monster.Name;
            cardButton.onClick.AddListener(() => ShowMonsterDetail(monster));
        }
        else
        {
            Debug.Log($"Setting up undiscovered monster card: {monster.name}");
            iconImage.color = Color.black; // Silhouette
            nameText.text = "???";
            cardButton.interactable = false;
        }
    }
    
    private void ShowMonsterDetail(EnemyStats monster)
    {
        Debug.Log($"Showing detail for monster: {monster.name}");
        
        if (monsterDetailPanel == null)
        {
            Debug.LogError("Detail panel is null!");
            return;
        }
        
        monsterDetailPanel.SetActive(true);
        
        if (detailImage == null || monsterNameText == null || descriptionText == null || statsText == null)
        {
            Debug.LogError("One or more detail view components are not assigned!");
            return;
        }
        
        // Use boss image for the detail view if it's a boss monster, otherwise use the regular icon
        Sprite displayImage = monster.IsSpecialBoss ? monster.BossImage : monster.MonsterIcon;
        if (displayImage == null) displayImage = monster.MonsterIcon; // Fall back to icon if boss image is missing
        
        if (displayImage == null)
        {
            Debug.LogError($"Detail image is missing for {monster.name}");
        }
        else
        {
            detailImage.sprite = displayImage;
        }
        
        monsterNameText.text = monster.Name;
        descriptionText.text = monster.Description;
        
        // Show different stats based on whether it's a boss or regular monster
        if (monster.IsSpecialBoss)
        {
            statsText.text = $"★ BOSS MONSTER ★\n\n" +
                            $"Health: {monster.HP}\n" +
                            $"Attack: {monster.ATK}\n" +
                            $"Defense: {monster.DEF}\n" +
                            $"Special Attack: {monster.SPA}\n" +
                            $"Special Defense: {monster.SPD}\n" +
                            $"Speed: {monster.SPE}\n" +
                            $"EXP Yield: {monster.ExpYield}";
        }
        else
        {
            statsText.text = $"Health: {monster.HP}\n" +
                            $"Attack: {monster.ATK}\n" +
                            $"Defense: {monster.DEF}\n" +
                            $"Special Attack: {monster.SPA}\n" +
                            $"Special Defense: {monster.SPD}\n" +
                            $"Speed: {monster.SPE}\n" +
                            $"EXP Yield: {monster.ExpYield}";
        }
                        
        Debug.Log("Monster detail view updated successfully");
    }
    
    // Call this method when a monster is defeated for the first time
    public void UnlockMonster(string monsterName)
    {
        Debug.Log($"[MonsterBookManager] Attempting to unlock monster: {monsterName}");
        
        // First try exact match by name
        EnemyStats foundMonster = null;
        
        // Try direct dictionary lookup if monsterName is an ID
        if (monsterDictionary.TryGetValue(monsterName, out foundMonster))
        {
            monsterDiscoveryStatus[monsterName] = true;
            Debug.Log($"[MonsterBookManager] Successfully unlocked monster by ID: {monsterName}");
        }
        else
        {
            // If not found as ID, try to match by display name
            foreach (var entry in monsterDictionary)
            {
                if (entry.Value.Name.Equals(monsterName, System.StringComparison.OrdinalIgnoreCase))
                {
                    monsterDiscoveryStatus[entry.Key] = true;
                    Debug.Log($"[MonsterBookManager] Successfully unlocked monster by name: {monsterName} (ID: {entry.Key})");
                    foundMonster = entry.Value;
                    break;
                }
            }
        }
        
        if (foundMonster != null)
        {
            // Save progress
            if (SimpleSaveSystem.Instance != null)
            {
                Debug.Log($"[MonsterBookManager] Saving game after discovering {monsterName}");
                SimpleSaveSystem.Instance.SaveGame();
            }
            else 
            {
                Debug.LogWarning($"[MonsterBookManager] SimpleSaveSystem.Instance is null! Can't save after discovering {monsterName}");
                
                // Try to find the SimpleSaveSystem in the scene
                SimpleSaveSystem saveSystem = FindObjectOfType<SimpleSaveSystem>();
                if (saveSystem != null)
                {
                    Debug.Log($"[MonsterBookManager] Found SimpleSaveSystem in scene, saving after discovering {monsterName}");
                    saveSystem.SaveGame();
                }
            }
            
            // Refresh the grid
            PopulateMonsterGrid();
        }
        else
        {
            Debug.LogError($"[MonsterBookManager] Failed to find monster with name: {monsterName}");
        }
    }
    
    // For debugging - discover all monsters at once
    public void DiscoverAllMonsters()
    {
        Debug.Log("Discovering all monsters...");
        
        foreach (var monsterId in monsterDictionary.Keys.ToList())
        {
            monsterDiscoveryStatus[monsterId] = true;
        }
        
        // Save progress
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.SaveGame();
        }
        
        // Refresh the grid
        PopulateMonsterGrid();
        
        Debug.Log($"All {monsterDictionary.Count} monsters discovered!");
    }
} 