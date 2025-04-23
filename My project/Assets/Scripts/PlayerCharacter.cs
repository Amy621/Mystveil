using UnityEngine;

/// <summary>
/// Component that provides access to player stats and character-related functionality
/// </summary>
public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private string playerName;
    [SerializeField] private Player player;

    // Runtime stats that can be modified
    private int currentHealth;
    private int maxHealth;
    private int currentMana;
    private int maxMana;
    private int level;
    private int attack;
    private int defense;
    private int specialAttack;
    private int specialDefense;
    private int speed;
    private int charisma;

    public PlayerStats GetPlayerStats()
    {
        return playerStats;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public Player GetPlayer()
    {
        return player;
    }

    private void Awake()
    {
        Debug.Log("PlayerCharacter.Awake() called");
        
        // If no player is assigned, try to find one
        if (player == null)
        {
            var playerDB = FindObjectOfType<PlayerDB>();
            if (playerDB != null)
            {
                player = playerDB.Player;
                Debug.Log("Found Player from PlayerDB");
            }
            else
            {
                Debug.LogWarning("No PlayerDB found in scene");
            }
        }

        // Initialize runtime stats from PlayerStats if available
        if (playerStats != null)
        {
            Debug.Log("Initializing stats from PlayerStats");
            currentHealth = playerStats.HP;
            maxHealth = playerStats.HP;
            currentMana = playerStats.MANA;
            maxMana = playerStats.MANA;
            level = 1;
            attack = playerStats.ATK;
            defense = playerStats.DEF;
            specialAttack = playerStats.SPA;
            specialDefense = playerStats.SPD;
            speed = playerStats.SPE;
            charisma = playerStats.CHA;
            
            Debug.Log($"Initialized stats - HP: {currentHealth}, MANA: {currentMana}, ATK: {attack}");
        }
        else
        {
            Debug.LogError("PlayerStats is not assigned in the Inspector!");
        }
    }

    // Properties for runtime stats
    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, maxHealth); }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public int CurrentMana
    {
        get { return currentMana; }
        set { currentMana = Mathf.Clamp(value, 0, maxMana); }
    }

    public int MaxMana
    {
        get { return maxMana; }
        set { maxMana = value; }
    }

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public int Attack
    {
        get { return attack; }
        set { attack = value; }
    }

    public int Defense
    {
        get { return defense; }
        set { defense = value; }
    }

    public int SpecialAttack
    {
        get { return specialAttack; }
        set { specialAttack = value; }
    }

    public int SpecialDefense
    {
        get { return specialDefense; }
        set { specialDefense = value; }
    }

    public int Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public int Charisma
    {
        get { return charisma; }
        set { charisma = value; }
    }
} 