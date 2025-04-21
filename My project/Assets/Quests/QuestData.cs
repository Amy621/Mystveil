using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyQuests
{
    [System.Serializable]
    public class ItemReward
    {
        public string itemID;
        public int quantity = 1;
    }

    [System.Serializable]
    public class QuestObjective
    {
        public string objectiveID;
        public string description;
        public string targetID;
        public int requiredAmount = 1;
        public int currentAmount = 0;
        public bool isCompleted = false;
        
        // Deep clone method
        public QuestObjective Clone()
        {
            return new QuestObjective
            {
                objectiveID = this.objectiveID,
                description = this.description,
                targetID = this.targetID,
                requiredAmount = this.requiredAmount,
                currentAmount = this.currentAmount,
                isCompleted = this.isCompleted
            };
        }
    }

    [CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest Data")]
    public class QuestData : ScriptableObject
    {
        [Header("Quest Info")]
        public string questName;
        public string description;
        public int levelRequirement = 1;
        
        [Header("Quest State")]
        [HideInInspector] public bool isActive = false;
        [HideInInspector] public bool isCompleted = false;
        
        [Header("Quest Objectives")]
        public List<QuestObjective> objectives = new List<QuestObjective>();
        
        [Header("Quest Rewards")]
        public int experienceReward = 100;
        public int goldReward = 50;
        public List<ItemReward> itemRewards = new List<ItemReward>();
        
        [Header("Quest Chain")]
        public QuestData nextQuest; // For quest chains
        
        [Header("Quest Type and Requirements")]
        public bool isMainQuest = false;
        public List<string> prerequisiteQuestNames = new List<string>();
        
        // Initialize quest when accepted
        public virtual void Initialize()
        {
            isActive = true;
            isCompleted = false;
            
            // Reset objective progress
            foreach (QuestObjective objective in objectives)
            {
                objective.currentAmount = 0;
                objective.isCompleted = false;
            }
        }
        
        // Check if all objectives are completed
        public bool CheckCompletion()
        {
            if (objectives.Count == 0)
                return true;
            
            foreach (QuestObjective objective in objectives)
            {
                if (!objective.isCompleted)
                    return false;
            }
            
            return true;
        }
        
        // Create a deep copy of this quest
        public QuestData Clone()
        {
            // Create a new instance
            QuestData clone = CreateInstance<QuestData>();
            
            // Copy basic properties
            clone.questName = this.questName;
            clone.description = this.description;
            clone.levelRequirement = this.levelRequirement;
            clone.isActive = this.isActive;
            clone.isCompleted = this.isCompleted;
            
            // Clone rewards
            clone.experienceReward = this.experienceReward;
            clone.goldReward = this.goldReward;
            
            // Clone item rewards
            clone.itemRewards = new List<ItemReward>();
            foreach (ItemReward item in this.itemRewards)
            {
                clone.itemRewards.Add(new ItemReward 
                { 
                    itemID = item.itemID, 
                    quantity = item.quantity 
                });
            }
            
            // Clone quest objectives
            clone.objectives = new List<QuestObjective>();
            foreach (QuestObjective obj in this.objectives)
            {
                clone.objectives.Add(obj.Clone());
            }
            
            // Reference the next quest
            clone.nextQuest = this.nextQuest;
            clone.isMainQuest = this.isMainQuest;
            
            // Copy prerequisite quests
            clone.prerequisiteQuestNames = new List<string>(this.prerequisiteQuestNames);
            
            return clone;
        }
        
        // Get an objective by ID
        public QuestObjective GetObjective(string objectiveID)
        {
            return objectives.Find(o => o.objectiveID == objectiveID);
        }
        
        // Calculate percentage of quest completion
        public float GetCompletionPercentage()
        {
            if (objectives.Count == 0)
                return 100f;
            
            int totalRequired = 0;
            int totalCurrent = 0;
            
            foreach (QuestObjective objective in objectives)
            {
                totalRequired += objective.requiredAmount;
                totalCurrent += Mathf.Min(objective.currentAmount, objective.requiredAmount);
            }
            
            return (float)totalCurrent / totalRequired * 100f;
        }
    }

    public enum ObjectiveType
    {
        KillEnemy,
        CollectItem,
        TalkToNPC,
        ReachLocation,
        CompleteQuest,
        UseItem,
        Custom
    }
}
