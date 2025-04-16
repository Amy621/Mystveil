using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Save/Load Menu")]
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject noSavesMessage;
    [SerializeField] private Transform saveSlotContainer;
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private GameObject loadingIndicator;
    
    private List<SaveSlotUI> saveSlotUIs = new List<SaveSlotUI>();
    private bool hasSaves = false;

    private void Awake()
    {
        // Ensure SaveManager is created
        if (SaveManager.Instance == null)
        {
            GameObject saveManagerObj = new GameObject("SaveManager");
            saveManagerObj.AddComponent<SaveManager>();
        }
    }

    private void Start()
    {
        // Set up button listeners
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
            
        if (newGameButton != null)
            newGameButton.onClick.AddListener(StartNewGame);
            
        if (backButton != null)
            backButton.onClick.AddListener(ShowMainMenu);
            
        // Initialize UI state
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
            
        if (saveLoadPanel != null)
            saveLoadPanel.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        // Show save/load menu
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
            
        if (saveLoadPanel != null)
        {
            saveLoadPanel.SetActive(true);
            
            // Check for save files
            CheckForSaveFiles();
        }
    }
    
    private void CheckForSaveFiles()
    {
        // Clear previous save slots
        foreach (var slotUI in saveSlotUIs)
        {
            Destroy(slotUI.gameObject);
        }
        saveSlotUIs.Clear();
        
        // Get save slots from SaveManager
        List<SaveSlot> saveSlots = SaveManager.Instance.GetSaveSlots();
        hasSaves = false;
        
        // Create UI for each save slot
        foreach (var saveSlot in saveSlots)
        {
            if (saveSlot.exists)
            {
                hasSaves = true;
                CreateSaveSlotUI(saveSlot);
            }
        }
        
        // Show appropriate UI based on save existence
        if (noSavesMessage != null)
            noSavesMessage.SetActive(!hasSaves);
            
        if (newGameButton != null)
            newGameButton.gameObject.SetActive(true);
    }
    
    private void CreateSaveSlotUI(SaveSlot saveSlot)
    {
        if (saveSlotPrefab == null || saveSlotContainer == null)
            return;
            
        GameObject slotObj = Instantiate(saveSlotPrefab, saveSlotContainer);
        SaveSlotUI slotUI = slotObj.GetComponent<SaveSlotUI>();
        
        if (slotUI != null)
        {
            slotUI.Init(saveSlot, OnSaveSlotSelected);
            saveSlotUIs.Add(slotUI);
        }
    }
    
    private void OnSaveSlotSelected(int slotIndex)
    {
        if (loadingIndicator != null)
            loadingIndicator.SetActive(true);
            
        // Load game data
        PlayerData playerData = SaveManager.Instance.LoadGame(slotIndex);
        
        if (playerData != null)
        {
            // Store the loaded data in a static variable or pass to the game scene
            GameManager.LoadedPlayerData = playerData;
            
            // Load the game scene
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogError("Failed to load save data from slot " + slotIndex);
            if (loadingIndicator != null)
                loadingIndicator.SetActive(false);
        }
    }
    
    private void StartNewGame()
    {
        // Create a new player data
        PlayerData newPlayerData = new PlayerData
        {
            playerName = "New Player",
            level = 1,
            health = 100,
            maxHealth = 100,
            experiencePoints = 0
        };
        
        // Store in static variable or save to a slot
        GameManager.LoadedPlayerData = newPlayerData;
        
        // Load the game scene
        SceneManager.LoadScene("GameScene");
    }
    
    private void ShowSettings()
    {
        // TODO: Implement settings menu
        Debug.Log("Settings button clicked");
    }
    
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
} 