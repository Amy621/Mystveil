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
    public int experiencePoints;
    
    // Player combat stats
    public int attackPoints;
    public int defensePoints;
    public int specialAttackPoints;
    public int specialDefensePoints;
    public int speed;
    
    // Player spells
    public List<string> equippedSpells = new List<string>();
    public List<string> unlockedSpells = new List<string>();
    
    // Player condition/status
    public string statusCondition;
    public int statusTime;
    public string volatileStatus;
    public int volatileStatusTime;
    
    // Save metadata
    public DateTime saveTime;
    
    // Codex data - for monsters, quests, spells, items, lore
    public List<string> unlockedBookTabs = new List<string>();
    public Dictionary<string, List<string>> codexStringLists = new Dictionary<string, List<string>>();
    public Dictionary<string, bool> codexDiscoveryFlags = new Dictionary<string, bool>();
    public Dictionary<string, int> codexIntValues = new Dictionary<string, int>();
    
    // Dictionary to store string values
    public Dictionary<string, string> codexStringValues = new Dictionary<string, string>();
    
    // Method to set a string list with a given key
    public void SetStringList(string key, List<string> values)
    {
        codexStringLists[key] = values;
    }
    
    // Method to get a string list with a given key
    public List<string> GetStringList(string key)
    {
        if (codexStringLists.TryGetValue(key, out List<string> result))
        {
            return result;
        }
        return null;
    }
    
    // Method to set a string array with a given key
    public void SetStringArray(string key, string[] values)
    {
        codexStringLists[key] = new List<string>(values);
    }
    
    // Method to get a string array with a given key
    public string[] GetStringArray(string key)
    {
        if (codexStringLists.TryGetValue(key, out List<string> result))
        {
            return result.ToArray();
        }
        return null;
    }
    
    // Method to set an integer value
    public void SetInt(string key, int value)
    {
        if (!codexIntValues.ContainsKey(key))
        {
            codexIntValues.Add(key, value);
        }
        else
        {
            codexIntValues[key] = value;
        }
    }
    
    // Method to get an integer value with a default if not found
    public int GetInt(string key, int defaultValue = 0)
    {
        if (codexIntValues.TryGetValue(key, out int value))
        {
            return value;
        }
        return defaultValue;
    }
    
    // Method to set a boolean value
    public void SetBool(string key, bool value)
    {
        codexDiscoveryFlags[key] = value;
    }
    
    // Method to get a boolean value with a default if not found
    public bool GetBool(string key, bool defaultValue = false)
    {
        if (codexDiscoveryFlags.TryGetValue(key, out bool value))
        {
            return value;
        }
        return defaultValue;
    }
    
    // Method to set a discovery flag
    public void SetDiscovered(string key, bool discovered)
    {
        codexDiscoveryFlags[key] = discovered;
    }
    
    // Method to check if something is discovered
    public bool IsDiscovered(string key)
    {
        if (codexDiscoveryFlags.TryGetValue(key, out bool result))
        {
            return result;
        }
        return false;
    }
    
    // Method to set a string value
    public void SetString(string key, string value)
    {
        codexStringValues[key] = value;
    }
    
    // Method to get a string value with a default if not found
    public string GetString(string key, string defaultValue = "")
    {
        if (codexStringValues.TryGetValue(key, out string value))
        {
            return value;
        }
        return defaultValue;
    }
} 