using UnityEngine;

/// <summary>
/// Adapter component that connects the player's mana system to the save system
/// </summary>
public class PlayerMana : MonoBehaviour
{
    [SerializeField] private int currentMana;
    [SerializeField] private int maxMana = 100;
    
    public int CurrentMana
    {
        get { return currentMana; }
        set 
        { 
            currentMana = Mathf.Clamp(value, 0, maxMana);
            OnManaChanged?.Invoke(currentMana);
        }
    }
    
    public int MaxMana
    {
        get { return maxMana; }
        set 
        { 
            maxMana = value;
            CurrentMana = Mathf.Min(CurrentMana, maxMana);  // Ensure current mana doesn't exceed max
            OnMaxManaChanged?.Invoke(maxMana);
        }
    }
    
    // Events for other components to listen to
    public delegate void ManaEvent(int value);
    public event ManaEvent OnManaChanged;
    public event ManaEvent OnMaxManaChanged;
    
    private void Awake()
    {
        // Check if we should initialize from PlayerStats
        var playerStats = GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            maxMana = playerStats.MANA;
        }
        
        // Initialize mana to max
        currentMana = maxMana;
    }
    
    public bool UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            CurrentMana -= amount;
            return true;
        }
        
        return false;  // Not enough mana
    }
    
    public void RestoreMana(int amount)
    {
        CurrentMana += amount;
    }
} 