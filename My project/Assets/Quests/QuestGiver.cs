using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegacyQuests
{
    public class QuestGiver : MonoBehaviour
    {
        [Header("Available Quests")]
        [SerializeField] private List<QuestData> availableQuests = new List<QuestData>();
        
        [Header("Quest Requirements")]
        [SerializeField] private int minPlayerLevel = 1;
        
        [Header("Interaction Settings")]
        [SerializeField] private float interactionDistance = 3f;
        [SerializeField] private GameObject questAvailableIndicator;
        [SerializeField] private GameObject questCompletableIndicator;
        
        [Header("Events")]
        public UnityEvent OnQuestOffered;
        public UnityEvent OnQuestAccepted;
        public UnityEvent OnQuestCompleted;
        
        private bool playerInRange = false;
        private Transform player;
        
        private void Start()
        {
            // Find player reference
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            
            // Initialize indicators
            UpdateQuestIndicators();
        }
        
        private void Update()
        {
            // Check if player is in range
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.position);
                playerInRange = distance <= interactionDistance;
                
                // Update indicators visibility based on distance
                UpdateQuestIndicators();
            }
        }
        
        // Update quest indicators
        private void UpdateQuestIndicators()
        {
            bool hasAvailableQuests = HasAvailableQuestsForPlayer();
            bool hasCompletableQuests = HasCompletableQuestsForPlayer();
            
            // Update visual indicators
            if (questAvailableIndicator != null)
                questAvailableIndicator.SetActive(playerInRange && hasAvailableQuests);
                
            if (questCompletableIndicator != null)
                questCompletableIndicator.SetActive(playerInRange && hasCompletableQuests);
        }
        
        // Open the quest dialog UI
        public void InteractWithQuestGiver()
        {
            if (!playerInRange)
                return;
                
            // Check if player has completable quests first
            if (HasCompletableQuestsForPlayer())
            {
                // Show turn-in dialog
                ShowQuestCompletionDialog();
            }
            else if (HasAvailableQuestsForPlayer())
            {
                // Show quest offering dialog
                ShowQuestOfferingDialog();
            }
            else
            {
                // Show generic dialog - no quests available
                ShowGenericDialog();
            }
        }
        
        // Check if there are available quests for the player
        private bool HasAvailableQuestsForPlayer()
        {
            if (availableQuests.Count == 0)
                return false;
                
            // Get player level
            PlayerLevel playerLevel = GetPlayerLevelComponent();
            int currentLevel = playerLevel != null ? playerLevel.CurrentLevel : 1;
            
            foreach (QuestData quest in availableQuests)
            {
                // Check if player meets level requirement
                if (currentLevel >= quest.levelRequirement)
                {
                    // Check if player doesn't already have this quest
                    if (!QuestManager.Instance.HasQuest(quest.questName))
                    {
                        // Check if player hasn't already completed this quest
                        if (!QuestManager.Instance.WasQuestCompleted(quest.questName))
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }
        
        // Check if player has any completable quests from this NPC
        private bool HasCompletableQuestsForPlayer()
        {
            foreach (QuestData quest in availableQuests)
            {
                // Check if quest is ready for turn-in
                if (QuestManager.Instance.IsQuestReadyForTurnIn(quest.questName))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        // Show dialog for offering available quests
        private void ShowQuestOfferingDialog()
        {
            // Get available quests for player
            List<QuestData> availableQuestsForPlayer = GetAvailableQuestsForPlayer();
            
            // TODO: Implement UI for quest offering
            Debug.Log($"Showing quest offering dialog with {availableQuestsForPlayer.Count} quests");
            
            // Trigger event
            OnQuestOffered?.Invoke();
            
            // For now, just accept the first available quest (replace with UI selection later)
            if (availableQuestsForPlayer.Count > 0)
            {
                AcceptQuest(availableQuestsForPlayer[0]);
            }
        }
        
        // Show dialog for turning in completable quests
        private void ShowQuestCompletionDialog()
        {
            // Get completable quests
            List<QuestData> completableQuests = GetCompletableQuests();
            
            // TODO: Implement UI for quest completion
            Debug.Log($"Showing quest completion dialog with {completableQuests.Count} quests");
            
            // For now, just complete the first completable quest (replace with UI selection later)
            if (completableQuests.Count > 0)
            {
                CompleteQuest(completableQuests[0].questName);
            }
        }
        
        // Show generic dialog when no quests are available
        private void ShowGenericDialog()
        {
            // TODO: Implement generic NPC dialog
            Debug.Log("No quests available from this NPC at the moment.");
        }
        
        // Get available quests for the player
        private List<QuestData> GetAvailableQuestsForPlayer()
        {
            List<QuestData> availableQuestsForPlayer = new List<QuestData>();
            
            // Get player level
            PlayerLevel playerLevel = GetPlayerLevelComponent();
            int currentLevel = playerLevel != null ? playerLevel.CurrentLevel : 1;
            
            foreach (QuestData quest in availableQuests)
            {
                // Check if player meets level requirement
                if (currentLevel >= quest.levelRequirement)
                {
                    // Check if player doesn't already have this quest
                    if (!QuestManager.Instance.HasQuest(quest.questName))
                    {
                        // Check if player hasn't already completed this quest
                        if (!QuestManager.Instance.WasQuestCompleted(quest.questName))
                        {
                            availableQuestsForPlayer.Add(quest);
                        }
                    }
                }
            }
            
            return availableQuestsForPlayer;
        }
        
        // Get quests that are ready to be turned in
        private List<QuestData> GetCompletableQuests()
        {
            List<QuestData> completableQuests = new List<QuestData>();
            
            foreach (QuestData questTemplate in availableQuests)
            {
                // Check if quest is ready for turn-in
                if (QuestManager.Instance.IsQuestReadyForTurnIn(questTemplate.questName))
                {
                    QuestData activeQuest = QuestManager.Instance.GetQuest(questTemplate.questName);
                    if (activeQuest != null)
                    {
                        completableQuests.Add(activeQuest);
                    }
                }
            }
            
            return completableQuests;
        }
        
        // Accept a quest
        public void AcceptQuest(QuestData quest)
        {
            QuestManager.Instance.AcceptQuest(quest);
            
            // Trigger event
            OnQuestAccepted?.Invoke();
            
            // Update indicators
            UpdateQuestIndicators();
            
            Debug.Log($"Quest accepted: {quest.questName}");
        }
        
        // Complete a quest
        public void CompleteQuest(string questName)
        {
            QuestManager.Instance.CompleteQuest(questName);
            
            // Trigger event
            OnQuestCompleted?.Invoke();
            
            // Update indicators
            UpdateQuestIndicators();
            
            Debug.Log($"Quest completed: {questName}");
        }
        
        // Helper method to get player level component
        private PlayerLevel GetPlayerLevelComponent()
        {
            if (player != null)
            {
                return player.GetComponent<PlayerLevel>();
            }
            return null;
        }
    }
} 