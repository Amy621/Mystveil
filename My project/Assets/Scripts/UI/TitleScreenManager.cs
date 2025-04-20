using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Save/Load Menu")]
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
        // Add button listeners
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
            
        if (backButton != null)
            backButton.onClick.AddListener(ShowMainMenu);
            
        if (newGameButton != null)
            newGameButton.onClick.AddListener(StartNewGame);
    }
    
    private void Start()
    {
        // Initialize save system if not already present
        if (SimpleSaveSystem.Instance == null)
        {
            Debug.LogWarning("SimpleSaveSystem not found in the scene. Create it using Tools > Save System > Create Simple Save System");
        }
        
        // Start with main menu active
        ShowMainMenu();
        
        // Check for save files
        CheckForSaveFiles();
    }
    
    public void ShowMainMenu()
    {
        // Toggle UI elements for main menu
        if (startButton != null)
            startButton.gameObject.SetActive(true);
            
        // Hide save slot UI
        foreach (SaveSlotUI slot in saveSlotUIs)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }
        saveSlotUIs.Clear();
    }
    
    public void OnStartButtonClicked()
    {
        // Show save slots or go directly to new game
        if (startButton != null)
            startButton.gameObject.SetActive(false);
            
        // If no saves, just start new game
        if (!hasSaves)
        {
            StartNewGame();
            return;
        }
        
        // Otherwise show save slots
        CheckForSaveFiles();
    }
    
    private void CheckForSaveFiles()
    {
        // Clear previous save slots
        foreach (SaveSlotUI slot in saveSlotUIs)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }
        saveSlotUIs.Clear();
        
        hasSaves = false;
        
        // Check if SimpleSaveSystem has a save
        string savePath = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                SimpleSaveData saveData = JsonUtility.FromJson<SimpleSaveData>(json);
                
                // Create a fake SaveSlot for compatibility
                SaveSlot saveSlot = new SaveSlot(0);
                saveSlot.UpdateFromSimpleSaveData(saveData);
                
                hasSaves = true;
                CreateSaveSlotUI(saveSlot);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading save file: " + e.Message);
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
            
        // Load game data using SimpleSaveSystem
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.LoadGame();
            
            // Since SimpleSaveSystem loads directly, we just need to switch to the game scene
            SceneManager.LoadScene(SimpleSaveSystem.Instance.GetLastLoadedScene());
        }
        else
        {
            Debug.LogError("SimpleSaveSystem not found when trying to load game");
            if (loadingIndicator != null)
                loadingIndicator.SetActive(false);
        }
    }
    
    private void StartNewGame()
    {
        // For new game, just load the starting scene
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