using System.Collections.Generic;
using UnityEngine;

namespace LegacyQuests
{
    public enum RewardType
    {
        Experience,
        Gold,
        Item
    }
    
    [System.Serializable]
    public class QuestReward
    {
        public RewardType rewardType;
        public int rewardAmount;
        public string rewardItemID;
    }
    
    [CreateAssetMenu(fileName = "GatherHerbsQuest", menuName = "Quests/Gather Herbs Quest")]
    public class GatherHerbsQuest : QuestData
    {
        public override void Initialize()
        {
            questName = "Gather Herbs";
            description = "Collect herbs for the town alchemist";
            isActive = false;
            isCompleted = false;
            
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    description = "Collect herbs",
                    currentAmount = 0,
                    requiredAmount = 5,
                    isCompleted = false
                },
                new QuestObjective
                {
                    description = "Return to the alchemist",
                    currentAmount = 0,
                    requiredAmount = 1,
                    isCompleted = false
                }
            };
            
            experienceReward = 100;
            goldReward = 50;
            itemRewards = new List<ItemReward>
            {
                new ItemReward
                {
                    itemID = "health_potion",
                    quantity = 3
                }
            };
        }
    }
} 