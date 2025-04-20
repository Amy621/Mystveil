using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Basic Info")]
    [SerializeField] private string playerName = "Hero";
    
    [Header("Stats")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private int charisma = 10;
    [SerializeField] private int level = 1;
    [SerializeField] private int experiencePoints = 0;
    [SerializeField] private int experienceToNextLevel = 100;
    [SerializeField] private int gold = 0;
    
    // Properties for save system
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int Charisma { get => charisma; set => charisma = value; }
    public int Level { get => level; set => level = value; }
    public int ExperiencePoints { get => experiencePoints; set => experiencePoints = value; }
    public int Gold { get => gold; set => gold = value; }
    
    // Events
    public delegate void PlayerStatsChangedEvent();
    public event PlayerStatsChangedEvent OnHealthChanged;
    public event PlayerStatsChangedEvent OnLevelChanged;
    public event PlayerStatsChangedEvent OnStatsChanged;
    
    private void Start()
    {
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
        }
    }
    
    // Get player name
    public string GetPlayerName()
    {
        return playerName;
    }
    
    // Set player name
    public void SetPlayerName(string name)
    {
        playerName = name;
    }
    
    // Take damage
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth < 0)
        {
            currentHealth = 0;
            Die();
        }
        
        OnHealthChanged?.Invoke();
    }
    
    // Heal player
    public void Heal(float amount)
    {
        currentHealth += amount;
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        OnHealthChanged?.Invoke();
    }
    
    // Player death
    private void Die()
    {
        Debug.Log("Player died!");
        // Death logic here
    }
    
    // Add experience
    public void AddExperience(int amount)
    {
        experiencePoints += amount;
        
        // Check for level up
        while (experiencePoints >= experienceToNextLevel)
        {
            LevelUp();
        }
    }
    
    // Level up
    private void LevelUp()
    {
        level++;
        experiencePoints -= experienceToNextLevel;
        
        // Increase experience needed for next level
        experienceToNextLevel = CalculateExperienceForLevel(level + 1);
        
        // Increase stats
        maxHealth += 10;
        currentHealth = maxHealth;
        
        Debug.Log($"Level up! Now level {level}");
        OnLevelChanged?.Invoke();
        
        // Save on level up
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.SaveGame();
        }
    }
    
    // Calculate experience needed for a specific level
    private int CalculateExperienceForLevel(int targetLevel)
    {
        // Simple formula: 100 * level^1.5
        return Mathf.RoundToInt(100 * Mathf.Pow(targetLevel, 1.5f));
    }
    
    // Add or remove gold
    public void AddGold(int amount)
    {
        gold += amount;
        
        if (gold < 0)
        {
            gold = 0;
        }
        
        OnStatsChanged?.Invoke();
    }
    
    // Increase charisma
    public void IncreaseCharisma(int amount)
    {
        charisma += amount;
        OnStatsChanged?.Invoke();
        
        // Save after charisma increase
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.SaveGame();
        }
    }
    
    // Get current stats as text
    public string GetStatsText()
    {
        return $"Level: {level}\n" +
               $"Health: {currentHealth}/{maxHealth}\n" +
               $"Experience: {experiencePoints}/{experienceToNextLevel}\n" +
               $"Charisma: {charisma}\n" +
               $"Gold: {gold}";
    }

    // Add this method to access the PlayerStats scriptable object
    public PlayerStats GetPlayerStats()
    {
        return playerStats;
    }
} 