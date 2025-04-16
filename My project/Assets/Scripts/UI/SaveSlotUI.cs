using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI playtimeText;
    [SerializeField] private TextMeshProUGUI lastSaveText;
    [SerializeField] private Button selectButton;
    
    private int slotIndex;
    private Action<int> onSelectCallback;
    
    public void Init(SaveSlot saveSlot, Action<int> callback)
    {
        slotIndex = saveSlot.slotIndex;
        onSelectCallback = callback;
        
        // Populate UI
        if (playerNameText != null)
            playerNameText.text = saveSlot.playerName;
            
        if (levelText != null)
            levelText.text = $"Level {saveSlot.level}";
            
        if (playtimeText != null)
            playtimeText.text = $"Playtime: {FormatPlaytime(saveSlot.playTime)}";
            
        if (lastSaveText != null)
            lastSaveText.text = $"Last Saved: {FormatDateTime(saveSlot.lastSaved)}";
            
        if (selectButton != null)
            selectButton.onClick.AddListener(OnSelectButtonClicked);
    }
    
    private void OnSelectButtonClicked()
    {
        onSelectCallback?.Invoke(slotIndex);
    }
    
    private string FormatPlaytime(float playTimeInSeconds)
    {
        int hours = (int)(playTimeInSeconds / 3600);
        int minutes = (int)((playTimeInSeconds % 3600) / 60);
        
        return $"{hours}h {minutes}m";
    }
    
    private string FormatDateTime(DateTime dateTime)
    {
        if (dateTime == DateTime.MinValue)
            return "Never";
            
        return dateTime.ToString("MMM d, yyyy h:mm tt");
    }
} 