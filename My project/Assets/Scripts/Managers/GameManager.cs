using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static PlayerData LoadedPlayerData { get; set; }
    
    [SerializeField] private bool hasCompletedGame = false;
    
    // Property for save system
    public bool HasCompletedGame 
    { 
        get => hasCompletedGame; 
        set 
        { 
            hasCompletedGame = value;
            if (value && SimpleSaveSystem.Instance != null)
            {
                SimpleSaveSystem.Instance.SaveGame();
            }
        } 
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Initialize systems
        if (SimpleSaveSystem.Instance == null)
        {
            Debug.LogWarning("SimpleSaveSystem not found. Make sure it's added to your title screen.");
            // SimpleSave system should be added to the title screen as a prefab
        }
    }
    
    // Call this when the player completes the game
    public void CompleteGame()
    {
        if (!hasCompletedGame)
        {
            hasCompletedGame = true;
            Debug.Log("Game completed!");
            
            // Trigger events or rewards for game completion
            UnlockCompletionRewards();
            
            // Save game completion
            if (SimpleSaveSystem.Instance != null)
            {
                SimpleSaveSystem.Instance.SaveGame();
            }
        }
    }
    
    // Unlock rewards for completing the game
    private void UnlockCompletionRewards()
    {
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        if (player != null)
        {
            // Example: Give experience
            player.AddExperience(5000);
            
            // Example: Give gold
            player.AddGold(10000);
            
            // Example: Unlock special items or spells
            SpellManager spellManager = player.GetComponent<SpellManager>();
            if (spellManager != null)
            {
                spellManager.UnlockSpell("spell_ultimate");
            }
            
            // Example: Unlock new game plus or special quests
            QuestManager questManager = FindObjectOfType<QuestManager>();
            if (questManager != null)
            {
                questManager.StartQuest("quest_newgameplus");
            }
        }
    }
    
    // Check if a player has completed the game
    public bool CheckGameCompletion()
    {
        return hasCompletedGame;
    }
    
    // New save game functionality
    public void SaveGame()
    {
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.SaveGame();
        }
    }
} 