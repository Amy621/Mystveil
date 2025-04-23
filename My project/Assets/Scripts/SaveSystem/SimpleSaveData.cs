using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SimpleSaveData
{
    // Player identity
    public string playerName;
    
    // Player position and scene
    public string currentScene;
    public float positionX;
    public float positionY;
    public float positionZ;
    
    // Player stats
    public int health;
    public int maxHealth;
    public int mana;
    public int maxMana;
    public int level;
    public int charisma;
    
    // Player combat stats
    public int attackPoints;
    public int defensePoints;
    public int specialAttackPoints;
    public int specialDefensePoints;
    public int speed;
    
    // Player spells
    public List<string> equippedSpells = new List<string>();
    public List<string> unlockedSpells = new List<string>();
    
    // Save metadata
    public DateTime saveTime;
} 