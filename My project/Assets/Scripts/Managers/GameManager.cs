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
            if (value && SaveManager.Instance != null)
            {
                SaveManager.Instance.SavePlayerData(PlayerPrefs.GetString("ActivePlayerID", "defaultPlayer"));
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
        if (SaveManager.Instance == null)
        {
            GameObject saveManagerObj = new GameObject("SaveManager");
            saveManagerObj.AddComponent<SaveManager>();
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
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SavePlayerData(PlayerPrefs.GetString("ActivePlayerID", "defaultPlayer"));
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
    public void SaveGame(int slotIndex)
    {
        if (LoadedPlayerData != null)
        {
            // Update any runtime data that might have changed
            // For example, playtime, current position, etc.
            
            SaveManager.Instance.SaveGame(slotIndex, LoadedPlayerData);
        }
    }
} 