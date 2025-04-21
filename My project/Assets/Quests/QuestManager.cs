using System.Collections.Generic;
using UnityEngine;
using System;

namespace LegacyQuests
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Active Quests")]
        public List<QuestData> activeQuests = new List<QuestData>();
        
        [Header("Completed Quests")]
        public List<QuestData> completedQuests = new List<QuestData>();

        // Events for quest state changes
        public event Action<QuestData> OnQuestAccepted;
        public event Action<QuestData> OnQuestCompleted;
        public event Action<QuestData, QuestObjective> OnObjectiveUpdated;
        public event Action<QuestData, QuestObjective> OnObjectiveCompleted;

        private void Awake()
        {
            // Singleton pattern
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

        // Accept a new quest
        public void AcceptQuest(QuestData quest)
        {
            if (quest == null)
                return;

            // Check if we already have this quest
            if (HasQuest(quest.questName))
            {
                Debug.LogWarning($"Quest '{quest.questName}' is already in the active quests list.");
                return;
            }

            // Clone the quest to avoid modifying the original ScriptableObject
            QuestData questInstance = quest.Clone();
            questInstance.isActive = true;
            questInstance.Initialize();
            
            activeQuests.Add(questInstance);
            
            // Notify listeners
            OnQuestAccepted?.Invoke(questInstance);
            
            Debug.Log($"Quest accepted: {quest.questName}");
        }

        // Check if a quest is already active
        public bool HasQuest(string questName)
        {
            return activeQuests.Exists(q => q.questName == questName);
        }

        // Get a quest by name
        public QuestData GetQuest(string questName)
        {
            return activeQuests.Find(q => q.questName == questName);
        }

        // Complete a quest
        public void CompleteQuest(string questName)
        {
            QuestData quest = GetQuest(questName);
            
            if (quest == null)
            {
                Debug.LogWarning($"Cannot complete quest '{questName}': Quest not found in active quests.");
                return;
            }

            // Mark as completed
            quest.isCompleted = true;
            
            // Move to completed quests list
            activeQuests.Remove(quest);
            completedQuests.Add(quest);
            
            // Notify listeners
            OnQuestCompleted?.Invoke(quest);
            
            // Give rewards
            GiveQuestRewards(quest);
            
            Debug.Log($"Quest completed: {quest.questName}");
        }

        // Check if a quest is completed and ready for turn-in
        public bool IsQuestReadyForTurnIn(string questName)
        {
            QuestData quest = GetQuest(questName);
            if (quest == null)
                return false;
                
            return quest.CheckCompletion();
        }

        // Update quest objective progress
        public void UpdateObjective(string questName, string targetID, int amount = 1)
        {
            QuestData quest = GetQuest(questName);
            
            if (quest == null)
            {
                Debug.LogWarning($"Cannot update objective: Quest '{questName}' not found in active quests.");
                return;
            }

            foreach (QuestObjective objective in quest.objectives)
            {
                // Skip completed objectives
                if (objective.isCompleted)
                    continue;
                    
                // Check if this objective matches the targetID
                if (objective.targetID == targetID)
                {
                    // Increase current amount
                    objective.currentAmount += amount;
                    
                    // Cap at required amount
                    if (objective.currentAmount >= objective.requiredAmount)
                    {
                        objective.currentAmount = objective.requiredAmount;
                        objective.isCompleted = true;
                        
                        // Notify listeners that objective is completed
                        OnObjectiveCompleted?.Invoke(quest, objective);
                        
                        // Check if all objectives are completed
                        if (quest.CheckCompletion())
                        {
                            Debug.Log($"All objectives for quest '{quest.questName}' completed!");
                            // Note: We don't auto-complete the quest here, as most games require the player to turn in the quest
                        }
                    }
                    else
                    {
                        // Notify listeners that objective was updated
                        OnObjectiveUpdated?.Invoke(quest, objective);
                    }
                    
                    break;
                }
            }
        }

        // Give rewards for completing a quest
        private void GiveQuestRewards(QuestData quest)
        {
            // Add experience
            if (quest.experienceReward > 0)
            {
                // Get player reference
                PlayerLevel playerLevel = FindObjectOfType<PlayerLevel>();
                if (playerLevel != null)
                {
                    playerLevel.AddExperience(quest.experienceReward);
                    Debug.Log($"Added {quest.experienceReward} experience to player");
                }
                else
                {
                    Debug.LogWarning("Cannot give experience reward: PlayerLevel component not found");
                }
            }
            
            // Add gold
            if (quest.goldReward > 0)
            {
                // Implement gold reward system
                Debug.Log($"Added {quest.goldReward} gold to player");
            }
            
            // Add items
            foreach (ItemReward itemReward in quest.itemRewards)
            {
                // Implement inventory system integration
                Debug.Log($"Added {itemReward.quantity} x {itemReward.itemID} to player inventory");
            }
        }

        // Check if a quest was completed before
        public bool WasQuestCompleted(string questName)
        {
            return completedQuests.Exists(q => q.questName == questName);
        }
    }
} 