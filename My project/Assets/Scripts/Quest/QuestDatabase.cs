using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Game/Databases/Quest Database")]
public class QuestDatabase : ScriptableObject
{
    [SerializeField] private List<GameQuest> quests = new List<GameQuest>();
    
    // Get quest by ID
    public GameQuest GetQuestByID(string questID)
    {
        return quests.Find(quest => quest.QuestID == questID);
    }
    
    // Get all quests
    public List<GameQuest> GetAllQuests()
    {
        return quests;
    }
}

// Placeholder Quest class
[System.Serializable]
public class GameQuest
{
    [SerializeField] private string questID;
    [SerializeField] private string questName;
    [SerializeField] private string description;
    [SerializeField] private int experienceReward;
    [SerializeField] private int goldReward;
    [SerializeField] private List<GameQuestObjective> objectives = new List<GameQuestObjective>();
    [SerializeField] private List<string> prerequisites = new List<string>();
    [SerializeField] private List<QuestItemReward> itemRewards = new List<QuestItemReward>();
    
    // Properties
    public string QuestID => questID;
    public string QuestName => questName;
    public string Description => description;
    public int ExperienceReward => experienceReward;
    public int GoldReward => goldReward;
    public List<GameQuestObjective> Objectives => objectives;
    public List<string> Prerequisites => prerequisites;
    public List<QuestItemReward> ItemRewards => itemRewards;
}

// Placeholder QuestObjective class
[System.Serializable]
public class GameQuestObjective
{
    [SerializeField] private string objectiveID;
    [SerializeField] private string description;
    [SerializeField] private int targetAmount = 1;
    
    // Properties
    public string ObjectiveID => objectiveID;
    public string Description => description;
    public int TargetAmount => targetAmount;
} 