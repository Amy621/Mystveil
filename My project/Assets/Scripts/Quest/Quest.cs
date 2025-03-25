using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string title;
    public string description;
    public string giver; // Who gave the quest
    public List<QuestObjective> objectives = new List<QuestObjective>();
    public List<QuestReward> rewards = new List<QuestReward>();
    public bool isCompleted;
    public bool isActive;
    public GameObject requiredItem;
}

[System.Serializable]
public class QuestObjective
{
    public string description;
    public int required;
    public int current;
    public bool isCompleted;
}

[System.Serializable]
public class QuestReward
{
    public string itemName;
    public Sprite itemIcon;
    public int quantity;
}
