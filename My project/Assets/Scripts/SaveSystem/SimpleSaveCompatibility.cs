using UnityEngine;
using System;
using System.Collections.Generic;

// Compatibility classes to help transition from the old save system
// These classes provide minimal implementations to fix compilation errors

/// <summary>
/// Compatibility class for the old SaveSlot type
/// </summary>
[System.Serializable]
public class SaveSlot
{
    public int slotIndex;
    public bool exists;
    public string playerName;
    public int level;
    public float playTime;
    public DateTime lastSaved;
    
    public SaveSlot(int index)
    {
        slotIndex = index;
        exists = false;
        playerName = "";
        level = 1;
        playTime = 0f;
        lastSaved = DateTime.MinValue;
    }
    
    // Update from SimpleSaveData
    public void UpdateFromSimpleSaveData(SimpleSaveData playerData)
    {
        exists = true;
        playerName = playerData.playerName;
        level = playerData.level;
        lastSaved = playerData.saveTime;
    }
}

/// <summary>
/// Compatibility class for the old PlayerData type
/// </summary>
[System.Serializable]
public class PlayerData
{
    // Basic player info
    public string playerName;
    public SimpleSerializableVector3 position;
    public float health;
    public float maxHealth;
    public int level;
    public int experiencePoints;
    
    // Constructor to create from SimpleSaveData
    public PlayerData() { }
    
    public PlayerData(SimpleSaveData data)
    {
        playerName = data.playerName;
        position = new SimpleSerializableVector3
        {
            x = data.positionX,
            y = data.positionY,
            z = data.positionZ
        };
        health = data.health;
        maxHealth = data.maxHealth;
        level = data.level;
    }
}

/// <summary>
/// Compatibility class for the old SerializableItem type
/// </summary>
[System.Serializable]
public class SerializableItem
{
    public string itemID;
    public int quantity;
    public int durability;
    public List<string> enchantments;
    
    public SerializableItem(string id, int qty, int dur, List<string> ench)
    {
        itemID = id;
        quantity = qty;
        durability = dur;
        enchantments = ench;
    }
}

/// <summary>
/// Compatibility class for the old QuestSaveData type
/// </summary>
[System.Serializable]
public class QuestSaveData
{
    public bool isActive;
    public bool isCompleted;
    public int currentStage;
    public Dictionary<string, int> objectives;
    
    public QuestSaveData(bool active, bool completed, int stage, Dictionary<string, int> obj)
    {
        isActive = active;
        isCompleted = completed;
        currentStage = stage;
        objectives = obj;
    }
} 