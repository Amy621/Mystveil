using UnityEngine;

/// <summary>
/// Adapter component that connects the player's health system to the save system
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth = 100;
    
    public int CurrentHealth
    {
        get { return currentHealth; }
        set 
        { 
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
    
    public int MaxHealth
    {
        get { return maxHealth; }
        set 
        { 
            maxHealth = value;
            CurrentHealth = Mathf.Min(CurrentHealth, maxHealth);  // Ensure current health doesn't exceed max
            OnMaxHealthChanged?.Invoke(maxHealth);
        }
    }
    
    // Events for other components to listen to
    public delegate void HealthEvent(int value);
    public event HealthEvent OnHealthChanged;
    public event HealthEvent OnMaxHealthChanged;
    
    private void Awake()
    {
        // Check if we should initialize from PlayerStats
        var playerStats = GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            maxHealth = playerStats.HP;
        }
        
        // Initialize health to max
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int amount)
    {
        CurrentHealth += amount;
    }
    
    private void Die()
    {
        Debug.Log("Player died!");
        // You could implement death behavior here or use an event to notify other systems
    }
} 