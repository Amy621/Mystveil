using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Manages the Quests page in the Enchanted Codex.
/// Displays active and completed quests, along with objectives and rewards.
/// </summary>
public class CodexQuestsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform activeQuestsContent;
    [SerializeField] private Transform completedQuestsContent;
    [SerializeField] private GameObject questEntryPrefab;
    
    [Header("Quest Details Panel")]
    [SerializeField] private GameObject questDetailsPanel;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private TextMeshProUGUI questObjectivesText;
    [SerializeField] private TextMeshProUGUI questRewardsText;
    [SerializeField] private TextMeshProUGUI questLocationText;
    [SerializeField] private Button closeDetailsButton;
    [SerializeField] private Button trackQuestButton;
    [SerializeField] private GameObject completedStamp;
    
    [Header("Tabs")]
    [SerializeField] private Button activeQuestsTab;
    [SerializeField] private Button completedQuestsTab;
    [SerializeField] private GameObject activeQuestsPanel;
    [SerializeField] private GameObject completedQuestsPanel;
    
    [Header("Settings")]
    [SerializeField] private Color activeQuestColor = new Color(1f, 0.9f, 0.6f);
    [SerializeField] private Color trackedQuestColor = new Color(0.6f, 1f, 0.6f);
    [SerializeField] private Color completedQuestColor = new Color(0.6f, 0.6f, 0.6f);
    [SerializeField] private Color failedQuestColor = new Color(1f, 0.6f, 0.6f);

    // Runtime data
    private Dictionary<string, QuestData> activeQuests = new Dictionary<string, QuestData>();
    private Dictionary<string, QuestData> completedQuests = new Dictionary<string, QuestData>();
    private List<GameObject> activeQuestObjects = new List<GameObject>();
    private List<GameObject> completedQuestObjects = new List<GameObject>();
    private string selectedQuestId;
    private string trackedQuestId;
    private bool initialized = false;
    private QuestManager questManager;

    /// <summary>
    /// Initializes the Quests page manager.
    /// </summary>
    public void Initialize()
    {
        if (initialized) return;
        
        // Find the quest manager
        questManager = FindObjectOfType<QuestManager>();
        if (questManager == null)
        {
            Debug.LogWarning("CodexQuestsManager: Could not find QuestManager!");
        }
        
        // Set up close button
        if (closeDetailsButton != null)
        {
            closeDetailsButton.onClick.RemoveAllListeners();
            closeDetailsButton.onClick.AddListener(CloseQuestDetails);
        }
        
        // Set up track button
        if (trackQuestButton != null)
        {
            trackQuestButton.onClick.RemoveAllListeners();
            trackQuestButton.onClick.AddListener(ToggleTrackSelectedQuest);
        }
        
        // Set up tabs
        if (activeQuestsTab != null)
        {
            activeQuestsTab.onClick.RemoveAllListeners();
            activeQuestsTab.onClick.AddListener(() => ShowTab(true));
        }
        
        if (completedQuestsTab != null)
        {
            completedQuestsTab.onClick.RemoveAllListeners();
            completedQuestsTab.onClick.AddListener(() => ShowTab(false));
        }
        
        // Initially show active quests
        ShowTab(true);
        
        // Hide details panel initially
        if (questDetailsPanel != null)
        {
            questDetailsPanel.SetActive(false);
        }
        
        // Subscribe to quest events if manager exists
        if (questManager != null)
        {
            questManager.OnQuestStarted += OnQuestStarted;
            questManager.OnQuestUpdated += OnQuestUpdated;
            questManager.OnQuestCompleted += OnQuestCompleted;
            questManager.OnQuestFailed += OnQuestFailed;
        }
        
        initialized = true;
        Debug.Log("CodexQuestsManager initialized");
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (questManager != null)
        {
            questManager.OnQuestStarted -= OnQuestStarted;
            questManager.OnQuestUpdated -= OnQuestUpdated;
            questManager.OnQuestCompleted -= OnQuestCompleted;
            questManager.OnQuestFailed -= OnQuestFailed;
        }
    }
    
    private void OnEnable()
    {
        // Refresh content when shown
        RefreshContent();
    }
    
    private void OnDisable()
    {
        // Close details panel if showing
        if (questDetailsPanel != null && questDetailsPanel.activeSelf)
        {
            questDetailsPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Switch between active and completed quest tabs.
    /// </summary>
    private void ShowTab(bool showActive)
    {
        if (activeQuestsPanel != null && completedQuestsPanel != null)
        {
            activeQuestsPanel.SetActive(showActive);
            completedQuestsPanel.SetActive(!showActive);
        }
        
        if (activeQuestsTab != null && completedQuestsTab != null)
        {
            activeQuestsTab.interactable = !showActive;
            completedQuestsTab.interactable = showActive;
        }
        
        // Close details when switching tabs
        if (questDetailsPanel != null && questDetailsPanel.activeSelf)
        {
            questDetailsPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Add a new quest to the active quests list.
    /// </summary>
    public void AddQuest(QuestData quest)
    {
        if (quest == null || activeQuests.ContainsKey(quest.questId))
            return;
            
        activeQuests[quest.questId] = quest;
        
        // If this page is currently visible, refresh it
        if (gameObject.activeInHierarchy)
        {
            RefreshContent();
        }
    }
    
    /// <summary>
    /// Update an existing quest with new data.
    /// </summary>
    public void UpdateQuest(QuestData quest)
    {
        if (quest == null) return;
        
        if (activeQuests.ContainsKey(quest.questId))
        {
            activeQuests[quest.questId] = quest;
            
            // If this page is currently visible, refresh it
            if (gameObject.activeInHierarchy)
            {
                RefreshContent();
                
                // If this is the selected quest, update details
                if (selectedQuestId == quest.questId && questDetailsPanel != null && questDetailsPanel.activeSelf)
                {
                    ShowQuestDetails(quest.questId);
                }
            }
        }
    }
    
    /// <summary>
    /// Mark a quest as completed and move it to the completed list.
    /// </summary>
    public void CompleteQuest(QuestData quest)
    {
        if (quest == null) return;
        
        // Remove from active quests
        if (activeQuests.ContainsKey(quest.questId))
        {
            activeQuests.Remove(quest.questId);
        }
        
        // Update quest status
        quest.status = QuestStatus.Completed;
        
        // Add to completed quests
        completedQuests[quest.questId] = quest;
        
        // If this was the tracked quest, clear tracking
        if (trackedQuestId == quest.questId)
        {
            trackedQuestId = null;
        }
        
        // If this page is currently visible, refresh it
        if (gameObject.activeInHierarchy)
        {
            RefreshContent();
        }
    }
    
    /// <summary>
    /// Mark a quest as failed and move it to the completed list.
    /// </summary>
    public void FailQuest(QuestData quest)
    {
        if (quest == null) return;
        
        // Remove from active quests
        if (activeQuests.ContainsKey(quest.questId))
        {
            activeQuests.Remove(quest.questId);
        }
        
        // Update quest status
        quest.status = QuestStatus.Failed;
        
        // Add to completed quests
        completedQuests[quest.questId] = quest;
        
        // If this was the tracked quest, clear tracking
        if (trackedQuestId == quest.questId)
        {
            trackedQuestId = null;
        }
        
        // If this page is currently visible, refresh it
        if (gameObject.activeInHierarchy)
        {
            RefreshContent();
        }
    }
    
    /// <summary>
    /// Refreshes the quests display.
    /// </summary>
    public void RefreshContent()
    {
        RefreshActiveQuests();
        RefreshCompletedQuests();
    }
    
    private void RefreshActiveQuests()
    {
        // Clear existing quest entries
        foreach (GameObject entryObj in activeQuestObjects)
        {
            Destroy(entryObj);
        }
        activeQuestObjects.Clear();
        
        // Sort quests by priority, then name
        List<QuestData> sortedQuests = new List<QuestData>(activeQuests.Values);
        sortedQuests.Sort((a, b) => {
            // Tracked quest first
            if (a.questId == trackedQuestId) return -1;
            if (b.questId == trackedQuestId) return 1;
            
            // Then by priority
            int priorityComparison = b.priority.CompareTo(a.priority);
            if (priorityComparison != 0) return priorityComparison;
            
            // Finally by name
            return a.title.CompareTo(b.title);
        });
        
        // Create an entry for each active quest
        foreach (QuestData quest in sortedQuests)
        {
            GameObject entryObj = Instantiate(questEntryPrefab, activeQuestsContent);
            activeQuestObjects.Add(entryObj);
            
            // Set up the entry
            Button button = entryObj.GetComponent<Button>();
            TextMeshProUGUI titleText = entryObj.GetComponentInChildren<TextMeshProUGUI>();
            
            if (titleText != null)
            {
                titleText.text = quest.title;
                
                // Add indicator for tracked quest
                if (quest.questId == trackedQuestId)
                {
                    titleText.text = "⭐ " + titleText.text;
                }
            }
            
            // Set color based on tracked status
            Image entryImage = entryObj.GetComponent<Image>();
            if (entryImage != null)
            {
                entryImage.color = (quest.questId == trackedQuestId) ? trackedQuestColor : activeQuestColor;
            }
            
            // Add listener to show quest details
            string id = quest.questId; // Capture for closure
            button.onClick.AddListener(() => ShowQuestDetails(id));
        }
    }
    
    private void RefreshCompletedQuests()
    {
        // Clear existing quest entries
        foreach (GameObject entryObj in completedQuestObjects)
        {
            Destroy(entryObj);
        }
        completedQuestObjects.Clear();
        
        // Sort quests by status, then completion time
        List<QuestData> sortedQuests = new List<QuestData>(completedQuests.Values);
        sortedQuests.Sort((a, b) => {
            // Failed quests at the bottom
            if (a.status != b.status)
            {
                if (a.status == QuestStatus.Failed) return 1;
                if (b.status == QuestStatus.Failed) return -1;
            }
            
            // Newer completions first
            return b.completionTime.CompareTo(a.completionTime);
        });
        
        // Create an entry for each completed quest
        foreach (QuestData quest in sortedQuests)
        {
            GameObject entryObj = Instantiate(questEntryPrefab, completedQuestsContent);
            completedQuestObjects.Add(entryObj);
            
            // Set up the entry
            Button button = entryObj.GetComponent<Button>();
            TextMeshProUGUI titleText = entryObj.GetComponentInChildren<TextMeshProUGUI>();
            
            if (titleText != null)
            {
                titleText.text = quest.title;
                
                // Add indicator for failed quests
                if (quest.status == QuestStatus.Failed)
                {
                    titleText.text = "❌ " + titleText.text;
                }
            }
            
            // Set color based on status
            Image entryImage = entryObj.GetComponent<Image>();
            if (entryImage != null)
            {
                entryImage.color = (quest.status == QuestStatus.Failed) ? failedQuestColor : completedQuestColor;
            }
            
            // Add listener to show quest details
            string id = quest.questId; // Capture for closure
            button.onClick.AddListener(() => ShowQuestDetails(id));
        }
    }
    
    private void ShowQuestDetails(string questId)
    {
        QuestData quest = null;
        
        // Look in both active and completed quests
        if (activeQuests.TryGetValue(questId, out quest) || completedQuests.TryGetValue(questId, out quest))
        {
            selectedQuestId = questId;
            
            // Make sure we have necessary components
            if (questDetailsPanel == null || questTitleText == null || questDescriptionText == null)
                return;
                
            // Show the details panel
            questDetailsPanel.SetActive(true);
            
            // Update UI elements
            questTitleText.text = quest.title;
            questDescriptionText.text = quest.description;
            
            // Build objectives text
            if (questObjectivesText != null)
            {
                string objectivesStr = "Objectives:\n";
                
                foreach (QuestObjective objective in quest.objectives)
                {
                    string status = objective.isCompleted ? "✓" : "□";
                    objectivesStr += $"{status} {objective.description}";
                    
                    if (objective.requiresCount)
                    {
                        objectivesStr += $" ({objective.currentCount}/{objective.requiredCount})";
                    }
                    
                    objectivesStr += "\n";
                }
                
                questObjectivesText.text = objectivesStr;
            }
            
            // Build rewards text
            if (questRewardsText != null)
            {
                string rewardsStr = "Rewards:\n";
                
                if (quest.rewardExp > 0)
                    rewardsStr += $"• {quest.rewardExp} XP\n";
                    
                if (quest.rewardGold > 0)
                    rewardsStr += $"• {quest.rewardGold} Gold\n";
                    
                if (quest.rewardItems != null && quest.rewardItems.Length > 0)
                {
                    foreach (ItemReward item in quest.rewardItems)
                    {
                        rewardsStr += $"• {item.itemName} x{item.quantity}\n";
                    }
                }
                
                questRewardsText.text = rewardsStr;
            }
            
            // Set location info
            if (questLocationText != null)
            {
                questLocationText.text = $"Location: {quest.location}";
            }
            
            // Show/hide completed stamp
            if (completedStamp != null)
            {
                completedStamp.SetActive(quest.status == QuestStatus.Completed);
            }
            
            // Update track button
            if (trackQuestButton != null)
            {
                // Only allow tracking active quests
                bool isActive = activeQuests.ContainsKey(questId);
                trackQuestButton.gameObject.SetActive(isActive);
                
                if (isActive)
                {
                    bool isTracked = (questId == trackedQuestId);
                    trackQuestButton.GetComponentInChildren<TextMeshProUGUI>().text = isTracked ? "Untrack" : "Track";
                }
            }
        }
    }
    
    private void CloseQuestDetails()
    {
        if (questDetailsPanel != null)
        {
            questDetailsPanel.SetActive(false);
        }
        selectedQuestId = null;
    }
    
    private void ToggleTrackSelectedQuest()
    {
        if (string.IsNullOrEmpty(selectedQuestId) || !activeQuests.ContainsKey(selectedQuestId))
            return;
            
        // Toggle tracking
        if (trackedQuestId == selectedQuestId)
        {
            trackedQuestId = null;
        }
        else
        {
            trackedQuestId = selectedQuestId;
        }
        
        // Update the UI
        RefreshContent();
        ShowQuestDetails(selectedQuestId);
        
        // Update quest manager
        if (questManager != null)
        {
            questManager.SetTrackedQuest(trackedQuestId);
        }
    }
    
    /// <summary>
    /// Handles saving quest data.
    /// </summary>
    public void OnSave(SimpleSaveData saveData)
    {
        // Most quest data is already saved by the QuestManager
        // We only need to save which quest is being tracked
        if (!string.IsNullOrEmpty(trackedQuestId))
        {
            saveData.SetString("codex_tracked_quest", trackedQuestId);
        }
        else
        {
            saveData.SetString("codex_tracked_quest", "");
        }
        
        Debug.Log("CodexQuestsManager: Saved tracked quest data");
    }
    
    /// <summary>
    /// Handles loading quest data.
    /// </summary>
    public void OnLoad(SimpleSaveData saveData)
    {
        // Clear current data
        activeQuests.Clear();
        completedQuests.Clear();
        
        // Get tracked quest
        trackedQuestId = saveData.GetString("codex_tracked_quest");
        if (string.IsNullOrEmpty(trackedQuestId))
        {
            trackedQuestId = null;
        }
        
        // Sync with the QuestManager to load all quests
        if (questManager != null)
        {
            try
            {
                // Load active quests
                foreach (QuestInstance questInstance in questManager.GetActiveQuests())
                {
                    QuestData questData = ConvertToQuestData(questInstance);
                    if (questData != null)
                    {
                        activeQuests[questData.questId] = questData;
                    }
                }
                
                // Load completed quests
                foreach (QuestInstance questInstance in questManager.GetCompletedQuests())
                {
                    QuestData questData = ConvertToQuestData(questInstance);
                    if (questData != null)
                    {
                        completedQuests[questData.questId] = questData;
                    }
                }
                
                // Set tracked quest in manager
                questManager.SetTrackedQuest(trackedQuestId);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading quests from QuestManager: {e.Message}");
            }
        }
        
        // Refresh UI
        RefreshContent();
        Debug.Log("CodexQuestsManager: Loaded quest data");
    }
    
    /// <summary>
    /// Quest status enum
    /// </summary>
    public enum QuestStatus
    {
        Active,
        Completed,
        Failed
    }
    
    /// <summary>
    /// Data class for quests.
    /// </summary>
    [System.Serializable]
    public class QuestData
    {
        public string questId;
        public string title;
        public string description;
        public string location;
        public int priority;
        public QuestStatus status = QuestStatus.Active;
        public QuestObjective[] objectives;
        public int rewardExp;
        public int rewardGold;
        public ItemReward[] rewardItems;
        public System.DateTime completionTime;
    }
    
    /// <summary>
    /// Data class for quest objectives.
    /// </summary>
    [System.Serializable]
    public class QuestObjective
    {
        public string description;
        public bool isCompleted;
        public bool requiresCount;
        public int currentCount;
        public int requiredCount;
    }
    
    /// <summary>
    /// Data class for item rewards.
    /// </summary>
    [System.Serializable]
    public class ItemReward
    {
        public string itemId;
        public string itemName;
        public int quantity;
    }
    
    // Event handlers to convert QuestInstance to QuestData
    private void OnQuestStarted(QuestInstance quest)
    {
        try
        {
            QuestData questData = ConvertToQuestData(quest);
            if (questData != null)
            {
                AddQuest(questData);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error converting QuestInstance to QuestData: {e.Message}");
        }
    }
    
    private void OnQuestUpdated(QuestInstance quest)
    {
        try
        {
            QuestData questData = ConvertToQuestData(quest);
            if (questData != null)
            {
                UpdateQuest(questData);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error converting QuestInstance to QuestData: {e.Message}");
        }
    }
    
    private void OnQuestCompleted(QuestInstance quest)
    {
        try
        {
            QuestData questData = ConvertToQuestData(quest);
            if (questData != null)
            {
                CompleteQuest(questData);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error converting QuestInstance to QuestData: {e.Message}");
        }
    }
    
    private void OnQuestFailed(QuestInstance quest)
    {
        try
        {
            QuestData questData = ConvertToQuestData(quest);
            if (questData != null)
            {
                FailQuest(questData);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error converting QuestInstance to QuestData: {e.Message}");
        }
    }
    
    private QuestData ConvertToQuestData(QuestInstance quest)
    {
        if (quest == null) return null;
        
        GameQuest gameQuest = quest.QuestData;
        if (gameQuest == null) return null;
        
        // Create objectives
        List<QuestObjective> objectives = new List<QuestObjective>();
        foreach (var objective in gameQuest.Objectives)
        {
            QuestObjective obj = new QuestObjective
            {
                description = objective.Description,
                requiresCount = objective.TargetAmount > 1,
                requiredCount = objective.TargetAmount,
                isCompleted = false
            };
            
            // Check if completed in the quest progress
            if (quest.ObjectiveProgress.TryGetValue(objective.ObjectiveID, out int progress))
            {
                obj.currentCount = progress;
                obj.isCompleted = progress >= objective.TargetAmount;
            }
            
            objectives.Add(obj);
        }
        
        // Create item rewards
        List<ItemReward> rewards = new List<ItemReward>();
        foreach (var reward in gameQuest.ItemRewards)
        {
            rewards.Add(new ItemReward
            {
                itemId = reward.ItemID,
                itemName = reward.ItemID, // Use ItemID as name since QuestItemReward doesn't have ItemName
                quantity = reward.Quantity
            });
        }
        
        // Create quest data
        QuestData questData = new QuestData
        {
            questId = gameQuest.QuestID,
            title = gameQuest.QuestName,
            description = gameQuest.Description,
            location = gameQuest.QuestID, // Use QuestID as location since GameQuest doesn't have Location
            priority = 0, // Default priority since GameQuest doesn't have Priority
            status = quest.IsCompleted ? QuestStatus.Completed : QuestStatus.Active,
            objectives = objectives.ToArray(),
            rewardExp = gameQuest.ExperienceReward,
            rewardGold = gameQuest.GoldReward,
            rewardItems = rewards.ToArray(),
            completionTime = System.DateTime.Now
        };
        
        return questData;
    }
} 