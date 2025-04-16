using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public class SerializableQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public SerializableQuaternion(Quaternion quaternion)
    {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}

[Serializable]
public class SerializableInventoryItem
{
    public string itemID;
    public int quantity;
    public int durability;
    public string[] enchantments;
    
    public SerializableInventoryItem(string id, int qty, int dur, string[] ench)
    {
        itemID = id;
        quantity = qty;
        durability = dur;
        enchantments = ench;
    }
}

[Serializable]
public class SerializableQuestProgress
{
    public string questId;
    public bool isActive;
    public bool isCompleted;
    public int currentStage;
    public SerializableObjective[] objectives;
    
    public SerializableQuestProgress(string id, bool active, bool completed, int stage, SerializableObjective[] obj)
    {
        questId = id;
        isActive = active;
        isCompleted = completed;
        currentStage = stage;
        objectives = obj;
    }
}

[Serializable]
public class SerializableObjective
{
    public string objectiveId;
    public int currentCount;
    
    public SerializableObjective(string id, int count)
    {
        objectiveId = id;
        currentCount = count;
    }
}

[Serializable]
public class SerializableEnemyDropStat
{
    public string enemyID;
    public float dropChance;
    
    public SerializableEnemyDropStat(string id, float chance)
    {
        enemyID = id;
        dropChance = chance;
    }
}

[Serializable]
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

[Serializable]
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