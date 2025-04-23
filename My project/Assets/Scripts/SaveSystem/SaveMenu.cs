using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI saveInfoText;
    
    private void Awake()
    {
        // Add button listeners
        SetupButtonListeners();
            
        // Hide menu at start
        gameObject.SetActive(false);
        
        UpdateSaveInfo();
    }
    
    private void OnEnable()
    {
        // Update save info when menu is opened
        UpdateSaveInfo();
    }
    
    // Public methods to set references (used by the prefab setup)
    public void SetSaveInfoText(TextMeshProUGUI text) => saveInfoText = text;
    public void SetSaveButton(Button button) => saveButton = button;
    public void SetLoadButton(Button button) => loadButton = button;
    public void SetCloseButton(Button button) => closeButton = button;
    
    private void SetupButtonListeners()
    {
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        
        if (loadButton != null)
            loadButton.onClick.AddListener(OnLoadButtonClicked);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseButtonClicked);
    }
    
    private void UpdateSaveInfo()
    {
        if (saveInfoText == null) return;
        
        string savePath = System.IO.Path.Combine(Application.persistentDataPath, "save.json");
        
        if (System.IO.File.Exists(savePath))
        {
            try
            {
                // Read save file
                string json = System.IO.File.ReadAllText(savePath);
                SimpleSaveData saveData = JsonUtility.FromJson<SimpleSaveData>(json);
                
                // Display save info
                saveInfoText.text = $"Last Save: {saveData.saveTime}\nScene: {saveData.currentScene}\nLevel: {saveData.level}";
            }
            catch
            {
                saveInfoText.text = "Save file exists but is corrupted.";
            }
        }
        else
        {
            saveInfoText.text = "No save file found.";
        }
    }
    
    public void OnSaveButtonClicked()
    {
        SimpleSaveSystem.Instance.SaveGame();
        UpdateSaveInfo();
    }
    
    public void OnLoadButtonClicked()
    {
        SimpleSaveSystem.Instance.LoadGame();
    }
    
    public void OnCloseButtonClicked()
    {
        SimpleSaveSystem.Instance.ToggleSaveMenu();
    }
} 