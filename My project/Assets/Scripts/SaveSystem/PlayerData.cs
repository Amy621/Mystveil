using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // Basic player info
    public string playerName;
    public SerializableVector3 position;
    public SerializableQuaternion rotation;
    
    // Stats
    public float health;
    public float maxHealth;
    public int charisma;
    
    // Level and Experience
    public int level;
    public int experiencePoints;
    
    // Game completion
    public bool hasCompletedGame;
    
    // Inventory
    public SerializableInventoryItem[] inventoryItems;
    
    // Quest progress
    public SerializableQuestProgress[] questProgress;
    
    // Spells
    public string[] unlockedSpells;
    public string[] equippedSpells;
    
    // Enemy drop stats
    public SerializableEnemyDropStat[] enemyDropStats;
} 