using UnityEngine;

/// <summary>
/// Adapter component that connects the player's level system to the save system
/// </summary>
public class PlayerLevel : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int[] expRequirements = { 0, 100, 300, 600, 1000, 1500, 2100, 2800, 3600, 4500 }; // Default exp requirements per level
    
    public int CurrentLevel
    {
        get { return currentLevel; }
        set 
        { 
            int oldLevel = currentLevel;
            currentLevel = Mathf.Max(1, value);  // Level can't be less than 1
            
            if (oldLevel != currentLevel)
            {
                OnLevelChanged?.Invoke(currentLevel);
            }
        }
    }
    
    public int CurrentExp
    {
        get { return currentExp; }
        set 
        { 
            currentExp = value;
            CheckLevelUp();
            OnExpChanged?.Invoke(currentExp);
        }
    }
    
    // Events for other components to listen to
    public delegate void LevelEvent(int value);
    public event LevelEvent OnLevelChanged;
    public event LevelEvent OnExpChanged;
    
    public void AddExperience(int amount)
    {
        CurrentExp += amount;
    }
    
    private void CheckLevelUp()
    {
        // Check if player has enough exp to level up
        while (currentLevel < expRequirements.Length - 1 && 
               currentExp >= GetExpRequiredForNextLevel())
        {
            CurrentLevel++;
            Debug.Log($"Level up! Now level {CurrentLevel}");
        }
    }
    
    public int GetExpRequiredForNextLevel()
    {
        if (currentLevel >= expRequirements.Length - 1)
        {
            return int.MaxValue; // Max level reached
        }
        
        return expRequirements[currentLevel];
    }
    
    public float GetLevelProgress()
    {
        int expForCurrentLevel = expRequirements[currentLevel - 1];
        int expForNextLevel = GetExpRequiredForNextLevel();
        int expInCurrentLevel = currentExp - expForCurrentLevel;
        int expRequiredForNextLevel = expForNextLevel - expForCurrentLevel;
        
        return (float)expInCurrentLevel / expRequiredForNextLevel;
    }
} 