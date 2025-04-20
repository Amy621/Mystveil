using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    
    [SerializeField] private QuestDatabase questDatabase;
    
    private Dictionary<string, QuestInstance> activeQuests = new Dictionary<string, QuestInstance>();
    private Dictionary<string, QuestInstance> completedQuests = new Dictionary<string, QuestInstance>();
    
    public delegate void QuestUpdatedEvent(QuestInstance quest);
    public event QuestUpdatedEvent OnQuestStarted;
    public event QuestUpdatedEvent OnQuestUpdated;
    public event QuestUpdatedEvent OnQuestCompleted;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start a new quest
    public bool StartQuest(string questID)
    {
        if (activeQuests.ContainsKey(questID) || completedQuests.ContainsKey(questID))
        {
            Debug.Log($"Quest {questID} already started or completed");
            return false;
        }
        
        GameQuest questData = questDatabase.GetQuestByID(questID);
        
        if (questData == null)
        {
            Debug.LogError($"Quest with ID {questID} not found in database");
            return false;
        }
        
        // Check prerequisites
        foreach (string prereqID in questData.Prerequisites)
        {
            if (!completedQuests.ContainsKey(prereqID))
            {
                Debug.Log($"Prerequisites not met for quest {questID}");
                return false;
            }
        }
        
        // Create new quest instance
        QuestInstance newQuest = new QuestInstance(questData);
        activeQuests.Add(questID, newQuest);
        
        Debug.Log($"Started quest: {questData.QuestName}");
        OnQuestStarted?.Invoke(newQuest);
        
        // When a quest starts, save player data
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.SaveGame();
        }
        
        return true;
    }
    
    // Update quest progress
    public void UpdateQuestProgress(string questID, string objectiveID, int amount = 1)
    {
        if (!activeQuests.ContainsKey(questID))
            return;
            
        QuestInstance quest = activeQuests[questID];
        bool updated = quest.UpdateObjective(objectiveID, amount);
        
        if (updated)
        {
            Debug.Log($"Updated quest {quest.QuestData.QuestName} objective");
            OnQuestUpdated?.Invoke(quest);
            
            // Check if quest is complete
            if (quest.IsCompleted)
            {
                CompleteQuest(questID);
            }
            
            // Save after any quest update
            if (SimpleSaveSystem.Instance != null)
            {
                SimpleSaveSystem.Instance.SaveGame();
            }
        }
    }
    
    // Complete a quest
    private void CompleteQuest(string questID)
    {
        if (!activeQuests.ContainsKey(questID))
            return;
            
        QuestInstance quest = activeQuests[questID];
        
        // Move from active to completed
        activeQuests.Remove(questID);
        completedQuests.Add(questID, quest);
        
        // Grant rewards
        GiveQuestRewards(quest);
        
        Debug.Log($"Completed quest: {quest.QuestData.QuestName}");
        OnQuestCompleted?.Invoke(quest);
        
        // Save after quest completion
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.SaveGame();
        }
    }
    
    // Give quest rewards
    private void GiveQuestRewards(QuestInstance quest)
    {
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        
        if (player != null)
        {
            // Give experience
            player.AddExperience(quest.QuestData.ExperienceReward);
            
            // Give gold
            player.AddGold(quest.QuestData.GoldReward);
            
            // Give items
            InventoryManager inventory = player.GetComponent<InventoryManager>();
            if (inventory != null)
            {
                foreach (QuestItemReward itemReward in quest.QuestData.ItemRewards)
                {
                    ItemDatabase itemDB = FindObjectOfType<ItemDatabase>();
                    if (itemDB != null)
                    {
                        GameItem item = itemDB.GetItemByID(itemReward.ItemID);
                        if (item != null)
                        {
                            inventory.AddItem(item, itemReward.Quantity);
                        }
                    }
                }
            }
        }
    }
    
    // Get active quests
    public List<QuestInstance> GetActiveQuests()
    {
        return activeQuests.Values.ToList();
    }
    
    // Get completed quests
    public List<QuestInstance> GetCompletedQuests()
    {
        return completedQuests.Values.ToList();
    }
    
    // Check if quest is active
    public bool IsQuestActive(string questID)
    {
        return activeQuests.ContainsKey(questID);
    }
    
    // Check if quest is completed
    public bool IsQuestCompleted(string questID)
    {
        return completedQuests.ContainsKey(questID);
    }
    
    // Get a specific quest instance
    public QuestInstance GetQuest(string questID)
    {
        if (activeQuests.ContainsKey(questID))
            return activeQuests[questID];
            
        if (completedQuests.ContainsKey(questID))
            return completedQuests[questID];
            
        return null;
    }
    
    // Get serializable quest data for saving
    public Dictionary<string, QuestSaveData> GetSerializableQuestProgress()
    {
        Dictionary<string, QuestSaveData> questProgress = new Dictionary<string, QuestSaveData>();
        
        // Save active quests
        foreach (var kvp in activeQuests)
        {
            QuestInstance quest = kvp.Value;
            questProgress.Add(kvp.Key, new QuestSaveData(
                true, // active
                false, // not completed
                quest.CurrentStage,
                quest.ObjectiveProgress
            ));
        }
        
        // Save completed quests
        foreach (var kvp in completedQuests)
        {
            QuestInstance quest = kvp.Value;
            questProgress.Add(kvp.Key, new QuestSaveData(
                false, // not active
                true, // completed
                quest.CurrentStage,
                quest.ObjectiveProgress
            ));
        }
        
        return questProgress;
    }
    
    // Load quest progress from serializable data
    public void LoadQuestProgress(Dictionary<string, QuestSaveData> questProgress)
    {
        // Clear current quest state
        activeQuests.Clear();
        completedQuests.Clear();
        
        foreach (var kvp in questProgress)
        {
            string questID = kvp.Key;
            QuestSaveData saveData = kvp.Value;
            
            GameQuest questData = questDatabase.GetQuestByID(questID);
            
            if (questData != null)
            {
                QuestInstance quest = new QuestInstance(questData);
                quest.CurrentStage = saveData.currentStage;
                quest.ObjectiveProgress = saveData.objectives;
                
                // Add to appropriate collection
                if (saveData.isCompleted)
                {
                    completedQuests.Add(questID, quest);
                }
                else if (saveData.isActive)
                {
                    activeQuests.Add(questID, quest);
                }
            }
            else
            {
                Debug.LogWarning($"Quest with ID {questID} not found in database during load");
            }
        }
        
        // Trigger quest updated events to refresh UI
        foreach (var quest in activeQuests.Values)
        {
            OnQuestStarted?.Invoke(quest);
        }
    }
    
    // Reset quests to default state
    public void ResetToDefault()
    {
        activeQuests.Clear();
        completedQuests.Clear();
        
        // Optional: Start tutorial or initial quests for new players
        StartQuest("quest_tutorial");
    }
}

[System.Serializable]
public class QuestInstance
{
    public GameQuest QuestData { get; private set; }
    public int CurrentStage { get; set; }
    public Dictionary<string, int> ObjectiveProgress { get; set; }
    public bool IsCompleted { get; private set; }
    
    public QuestInstance(GameQuest questData)
    {
        QuestData = questData;
        CurrentStage = 0;
        ObjectiveProgress = new Dictionary<string, int>();
        IsCompleted = false;
        
        // Initialize objectives
        foreach (GameQuestObjective objective in questData.Objectives)
        {
            ObjectiveProgress[objective.ObjectiveID] = 0;
        }
    }
    
    public bool UpdateObjective(string objectiveID, int amount)
    {
        if (!ObjectiveProgress.ContainsKey(objectiveID))
            return false;
            
        // Find the objective data
        GameQuestObjective objective = QuestData.Objectives.Find(o => o.ObjectiveID == objectiveID);
        
        if (objective != null)
        {
            // Update progress
            ObjectiveProgress[objectiveID] += amount;
            
            // Cap at target
            if (ObjectiveProgress[objectiveID] > objective.TargetAmount)
            {
                ObjectiveProgress[objectiveID] = objective.TargetAmount;
            }
            
            // Check if all objectives are complete
            CheckCompletion();
            
            return true;
        }
        
        return false;
    }
    
    private void CheckCompletion()
    {
        foreach (GameQuestObjective objective in QuestData.Objectives)
        {
            if (ObjectiveProgress[objective.ObjectiveID] < objective.TargetAmount)
            {
                // If any objective is incomplete, quest is not complete
                IsCompleted = false;
                return;
            }
        }
        
        // All objectives complete
        IsCompleted = true;
    }
} 