using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Save Settings")]
    [SerializeField] private float autoSaveInterval = 300f; // 5 minutes in seconds
    [SerializeField] private bool enableAutoSave = true;
    [SerializeField] private bool saveOnQuit = true;
    [SerializeField] private bool saveOnQuestUpdate = true;

    [SerializeField] private int maxSaveSlots = 3;
    [SerializeField] private string saveFolder = "Saves";
    [SerializeField] private string saveExtension = ".sav";

    private float autoSaveTimer;
    private bool isLoading = false;
    
    private string savePath;
    private List<SaveSlot> saveSlots = new List<SaveSlot>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSaveSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSaveSystem()
    {
        savePath = Path.Combine(Application.persistentDataPath, saveFolder);
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        
        // Initialize save slots
        for (int i = 0; i < maxSaveSlots; i++)
        {
            saveSlots.Add(new SaveSlot(i));
        }
        
        LoadSaveSlots();
    }

    private void Start()
    {
        autoSaveTimer = autoSaveInterval;
    }

    private void Update()
    {
        if (enableAutoSave)
        {
            autoSaveTimer -= Time.deltaTime;
            if (autoSaveTimer <= 0f)
            {
                SavePlayerData(GetActivePlayerID());
                autoSaveTimer = autoSaveInterval;
                Debug.Log("Auto-save completed");
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (saveOnQuit)
        {
            SavePlayerData(GetActivePlayerID());
            Debug.Log("Save on quit completed");
        }
    }

    // Return the currently active player ID - in a real MMO, this would come from the login system
    private string GetActivePlayerID()
    {
        // For testing, we'll use a default player ID, but in production this would be dynamic
        return PlayerPrefs.GetString("ActivePlayerID", "defaultPlayer");
    }

    // Call this when the player logs in
    public void OnPlayerLogin(string playerID)
    {
        Debug.Log($"Player {playerID} logged in, loading data...");
        LoadPlayerData(playerID);
        PlayerPrefs.SetString("ActivePlayerID", playerID);
    }

    // Call this when the player logs out
    public void OnPlayerLogout(string playerID)
    {
        Debug.Log($"Player {playerID} logged out, saving data...");
        SavePlayerData(playerID);
        PlayerPrefs.DeleteKey("ActivePlayerID");
    }

    // Call this whenever a quest updates
    public void OnQuestUpdated()
    {
        if (saveOnQuestUpdate)
        {
            SavePlayerData(GetActivePlayerID());
            Debug.Log("Save on quest update completed");
        }
    }

    // Main method to save player data
    public void SavePlayerData(string playerID)
    {
        try
        {
            PlayerData data = CollectPlayerData();
            string filePath = Path.Combine(savePath, $"{playerID}{saveExtension}");

            using (FileStream file = File.Create(filePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, data);
            }
            
            Debug.Log($"Player data saved successfully for {playerID}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save player data: {e.Message}");
        }
    }

    // Main method to load player data
    public void LoadPlayerData(string playerID)
    {
        if (isLoading) return; // Prevent concurrent loads
        
        isLoading = true;
        string filePath = Path.Combine(savePath, $"{playerID}{saveExtension}");
        
        try
        {
            if (File.Exists(filePath))
            {
                using (FileStream file = File.Open(filePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    PlayerData data = (PlayerData)formatter.Deserialize(file);
                    ApplyPlayerData(data);
                }
                
                Debug.Log($"Player data loaded successfully for {playerID}");
            }
            else
            {
                Debug.Log($"No save file found for {playerID}, creating new player data");
                CreateNewPlayerData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load player data: {e.Message}");
            CreateNewPlayerData();
        }
        finally
        {
            isLoading = false;
        }
    }

    // Collect all player data from various game systems
    private PlayerData CollectPlayerData()
    {
        PlayerData data = new PlayerData();
        
        // Get player character reference
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        
        if (player != null)
        {
            // Basic stats
            data.playerName = player.GetPlayerName();
            data.position = new SerializableVector3(player.transform.position);
            data.rotation = new SerializableQuaternion(player.transform.rotation);
            data.health = player.CurrentHealth;
            data.maxHealth = player.MaxHealth;
            data.charisma = player.Charisma;
            
            // Level and Experience
            data.level = player.Level;
            data.experiencePoints = player.ExperiencePoints;
            
            // Game completion
            data.hasCompletedGame = GameManager.Instance != null ? GameManager.Instance.HasCompletedGame : false;
            
            // Inventory
            InventoryManager inventory = player.GetComponent<InventoryManager>();
            if (inventory != null)
            {
                // Convert SerializableItem list to SerializableInventoryItem array
                List<SerializableItem> itemsList = inventory.GetSerializableInventory();
                SerializableInventoryItem[] inventoryItems = new SerializableInventoryItem[itemsList.Count];
                
                for (int i = 0; i < itemsList.Count; i++)
                {
                    SerializableItem item = itemsList[i];
                    // Convert SerializableItem to SerializableInventoryItem
                    inventoryItems[i] = new SerializableInventoryItem(
                        item.itemID, 
                        item.quantity, 
                        item.durability, 
                        item.enchantments.ToArray()
                    );
                }
                
                data.inventoryItems = inventoryItems;
            }
            
            // Quest progress
            QuestManager questManager = FindObjectOfType<QuestManager>();
            if (questManager != null)
            {
                Dictionary<string, QuestSaveData> questData = questManager.GetSerializableQuestProgress();
                int count = questData.Count;
                data.questProgress = new SerializableQuestProgress[count];
                
                int index = 0;
                foreach (var kvp in questData)
                {
                    QuestSaveData qData = kvp.Value;
                    
                    // Convert Dictionary to array for serialization
                    int objectiveCount = qData.objectives.Count;
                    SerializableObjective[] objectives = new SerializableObjective[objectiveCount];
                    
                    int objIndex = 0;
                    foreach (var objKvp in qData.objectives)
                    {
                        objectives[objIndex] = new SerializableObjective(objKvp.Key, objKvp.Value);
                        objIndex++;
                    }
                    
                    data.questProgress[index] = new SerializableQuestProgress(
                        kvp.Key,
                        qData.isActive,
                        qData.isCompleted,
                        qData.currentStage,
                        objectives
                    );
                    
                    index++;
                }
            }
            
            // Spell states
            SpellManager spellManager = player.GetComponent<SpellManager>();
            if (spellManager != null)
            {
                // Convert Lists to arrays
                data.unlockedSpells = spellManager.GetUnlockedSpells().ToArray();
                data.equippedSpells = spellManager.GetEquippedSpells().ToArray();
            }
            
            // Enemy drop stats
            EnemyDropManager dropManager = FindObjectOfType<EnemyDropManager>();
            if (dropManager != null)
            {
                // Convert Dictionary to array
                Dictionary<string, float> enemyDrops = dropManager.GetEnemyDropStats();
                SerializableEnemyDropStat[] enemyDropStats = new SerializableEnemyDropStat[enemyDrops.Count];
                
                int index = 0;
                foreach (var kvp in enemyDrops)
                {
                    enemyDropStats[index] = new SerializableEnemyDropStat(kvp.Key, kvp.Value);
                    index++;
                }
                
                data.enemyDropStats = enemyDropStats;
            }
        }
        else
        {
            Debug.LogWarning("Player character not found when saving data");
        }
        
        return data;
    }

    // Apply loaded data to game systems
    private void ApplyPlayerData(PlayerData data)
    {
        // Get player character reference
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        
        if (player != null)
        {
            // Basic stats
            player.SetPlayerName(data.playerName);
            player.transform.position = data.position.ToVector3();
            player.transform.rotation = data.rotation.ToQuaternion();
            player.CurrentHealth = data.health;
            player.MaxHealth = data.maxHealth;
            player.Charisma = data.charisma;
            
            // Level and Experience
            player.Level = data.level;
            player.ExperiencePoints = data.experiencePoints;
            
            // Game completion
            if (GameManager.Instance != null)
            {
                GameManager.Instance.HasCompletedGame = data.hasCompletedGame;
            }
            
            // Inventory
            InventoryManager inventory = player.GetComponent<InventoryManager>();
            if (inventory != null && data.inventoryItems != null)
            {
                // Convert SerializableInventoryItem array to SerializableItem list
                List<SerializableItem> items = new List<SerializableItem>();
                
                foreach (SerializableInventoryItem invItem in data.inventoryItems)
                {
                    // Convert SerializableInventoryItem to SerializableItem
                    items.Add(new SerializableItem(
                        invItem.itemID,
                        invItem.quantity,
                        invItem.durability,
                        new List<string>(invItem.enchantments)
                    ));
                }
                
                inventory.LoadInventory(items);
            }
            
            // Quest progress
            QuestManager questManager = FindObjectOfType<QuestManager>();
            if (questManager != null && data.questProgress != null)
            {
                Dictionary<string, QuestSaveData> questData = new Dictionary<string, QuestSaveData>();
                
                foreach (SerializableQuestProgress quest in data.questProgress)
                {
                    // Convert array to dictionary for quest system
                    Dictionary<string, int> objectives = new Dictionary<string, int>();
                    
                    foreach (SerializableObjective obj in quest.objectives)
                    {
                        objectives[obj.objectiveId] = obj.currentCount;
                    }
                    
                    questData[quest.questId] = new QuestSaveData(
                        quest.isActive,
                        quest.isCompleted,
                        quest.currentStage,
                        objectives
                    );
                }
                
                questManager.LoadQuestProgress(questData);
            }
            
            // Spell states
            SpellManager spellManager = player.GetComponent<SpellManager>();
            if (spellManager != null)
            {
                if (data.unlockedSpells != null)
                    spellManager.LoadUnlockedSpells(new List<string>(data.unlockedSpells));
                
                if (data.equippedSpells != null)
                    spellManager.LoadEquippedSpells(new List<string>(data.equippedSpells));
            }
            
            // Enemy drop stats
            EnemyDropManager dropManager = FindObjectOfType<EnemyDropManager>();
            if (dropManager != null && data.enemyDropStats != null)
            {
                // Convert array to Dictionary
                Dictionary<string, float> enemyDrops = new Dictionary<string, float>();
                
                foreach (SerializableEnemyDropStat stat in data.enemyDropStats)
                {
                    enemyDrops[stat.enemyID] = stat.dropChance;
                }
                
                dropManager.LoadEnemyDropStats(enemyDrops);
            }
        }
        else
        {
            Debug.LogWarning("Player character not found when loading data");
        }
    }

    // Create default data for a new player
    private void CreateNewPlayerData()
    {
        // Get player character reference
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        
        if (player != null)
        {
            // Reset to default values
            player.CurrentHealth = player.MaxHealth;
            player.Level = 1;
            player.ExperiencePoints = 0;
            player.Charisma = 10; // Default charisma
            
            // Reset inventory, quests, spells, etc. to their default new player state
            InventoryManager inventory = player.GetComponent<InventoryManager>();
            if (inventory != null)
            {
                inventory.ResetToDefault();
            }
            
            QuestManager questManager = FindObjectOfType<QuestManager>();
            if (questManager != null)
            {
                questManager.ResetToDefault();
            }
            
            SpellManager spellManager = player.GetComponent<SpellManager>();
            if (spellManager != null)
            {
                spellManager.ResetToDefault();
            }
            
            Debug.Log("Created new player data");
        }
    }

    public void LoadSaveSlots()
    {
        for (int i = 0; i < maxSaveSlots; i++)
        {
            string savePath = GetSavePath(i);
            if (File.Exists(savePath))
            {
                try
                {
                    string json = File.ReadAllText(savePath);
                    PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
                    
                    if (playerData != null)
                    {
                        saveSlots[i].UpdateFromPlayerData(playerData);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading save slot {i}: {e.Message}");
                }
            }
        }
    }
    
    public List<SaveSlot> GetSaveSlots()
    {
        return saveSlots;
    }
    
    public void SaveGame(int slotIndex, PlayerData playerData)
    {
        if (slotIndex < 0 || slotIndex >= maxSaveSlots)
        {
            Debug.LogError($"Invalid save slot index: {slotIndex}");
            return;
        }
        
        try
        {
            string json = JsonUtility.ToJson(playerData, true);
            string savePath = GetSavePath(slotIndex);
            File.WriteAllText(savePath, json);
            
            // Update save slot info
            saveSlots[slotIndex].UpdateFromPlayerData(playerData);
            
            Debug.Log($"Game saved successfully in slot {slotIndex}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}");
        }
    }
    
    public PlayerData LoadGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSaveSlots)
        {
            Debug.LogError($"Invalid save slot index: {slotIndex}");
            return null;
        }
        
        string savePath = GetSavePath(slotIndex);
        if (!File.Exists(savePath))
        {
            Debug.LogWarning($"No save file found in slot {slotIndex}");
            return null;
        }
        
        try
        {
            string json = File.ReadAllText(savePath);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            
            if (playerData != null)
            {
                Debug.Log($"Game loaded successfully from slot {slotIndex}");
                return playerData;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading game: {e.Message}");
        }
        
        return null;
    }
    
    public void DeleteSave(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSaveSlots)
        {
            Debug.LogError($"Invalid save slot index: {slotIndex}");
            return;
        }
        
        string savePath = GetSavePath(slotIndex);
        if (File.Exists(savePath))
        {
            try
            {
                File.Delete(savePath);
                saveSlots[slotIndex] = new SaveSlot(slotIndex);
                Debug.Log($"Save deleted successfully from slot {slotIndex}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error deleting save: {e.Message}");
            }
        }
    }
    
    private string GetSavePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, saveFolder, $"save{slotIndex}{saveExtension}");
    }
}

[System.Serializable]
public class SaveSlot
{
    public int slotIndex;
    public bool exists;
    public string playerName;
    public int level;
    public float playTime;
    public System.DateTime lastSaved;

    public SaveSlot(int index)
    {
        slotIndex = index;
        exists = false;
        lastSaved = System.DateTime.MinValue;
    }

    public void UpdateFromPlayerData(PlayerData playerData)
    {
        playerName = playerData.playerName;
        level = playerData.level;
        playTime = Time.timeSinceLevelLoad;
        lastSaved = System.DateTime.Now;
        exists = true;
    }
} 