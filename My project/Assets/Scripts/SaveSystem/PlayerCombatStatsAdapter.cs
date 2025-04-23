using UnityEngine;

/// <summary>
/// Adapter component that connects the player's combat stats to the save system
/// </summary>
public class PlayerCombatStatsAdapter : MonoBehaviour
{
    [SerializeField] private int attackPoints;
    [SerializeField] private int defensePoints;
    [SerializeField] private int specialAttackPoints;
    [SerializeField] private int specialDefensePoints;
    [SerializeField] private int speed;
    
    // Properties with getters and setters
    public int AttackPoints
    {
        get { return attackPoints; }
        set 
        { 
            attackPoints = value;
            OnStatsChanged?.Invoke();
        }
    }
    
    public int DefensePoints
    {
        get { return defensePoints; }
        set 
        { 
            defensePoints = value;
            OnStatsChanged?.Invoke();
        }
    }
    
    public int SpecialAttackPoints
    {
        get { return specialAttackPoints; }
        set 
        { 
            specialAttackPoints = value;
            OnStatsChanged?.Invoke();
        }
    }
    
    public int SpecialDefensePoints
    {
        get { return specialDefensePoints; }
        set 
        { 
            specialDefensePoints = value;
            OnStatsChanged?.Invoke();
        }
    }
    
    public int Speed
    {
        get { return speed; }
        set 
        { 
            speed = value;
            OnStatsChanged?.Invoke();
        }
    }
    
    // Event for when stats change
    public delegate void StatsChangedEvent();
    public event StatsChangedEvent OnStatsChanged;
    
    private void Awake()
    {
        // Check if we should initialize from PlayerStats
        var playerStats = GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            attackPoints = playerStats.ATK;
            defensePoints = playerStats.DEF;
            specialAttackPoints = playerStats.SPA;
            specialDefensePoints = playerStats.SPD;
            speed = playerStats.SPE;
        }
    }
} 