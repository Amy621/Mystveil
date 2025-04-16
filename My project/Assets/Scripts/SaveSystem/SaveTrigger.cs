using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This component allows you to trigger a save at specific points or areas in the game.
/// Attach to an object with a collider (set to trigger) to create save points.
/// </summary>
public class SaveTrigger : MonoBehaviour
{
    [Header("Save Settings")]
    [SerializeField] private TriggerType triggerType = TriggerType.EnterTrigger;
    [SerializeField] private bool showSaveMessage = true;
    [SerializeField] private float messageDisplayTime = 2f;
    [SerializeField] private string saveMessage = "Game Saved";
    
    [Header("Restrictions")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float cooldownTime = 5f;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onSave;
    
    private float lastSaveTime;
    private UIManager uiManager;
    
    public enum TriggerType
    {
        EnterTrigger,
        ExitTrigger,
        StayInTrigger,
        Manual
    }
    
    private void Start()
    {
        // Try to find UI Manager
        uiManager = FindObjectOfType<UIManager>();
        
        // Make sure we have a collider
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger && triggerType != TriggerType.Manual)
        {
            Debug.LogWarning("SaveTrigger requires a trigger collider to work properly. Setting isTrigger to true.");
            col.isTrigger = true;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (triggerType == TriggerType.EnterTrigger && other.CompareTag(playerTag))
        {
            TriggerSave();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (triggerType == TriggerType.ExitTrigger && other.CompareTag(playerTag))
        {
            TriggerSave();
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (triggerType == TriggerType.StayInTrigger && other.CompareTag(playerTag))
        {
            // Only save once per cooldown period
            if (Time.time - lastSaveTime >= cooldownTime)
            {
                TriggerSave();
            }
        }
    }
    
    /// <summary>
    /// Manually trigger the save. Can be called from other scripts or from UI buttons.
    /// </summary>
    public void TriggerSave()
    {
        // Check cooldown
        if (Time.time - lastSaveTime < cooldownTime)
        {
            return;
        }
        
        // Save the game
        if (SaveManager.Instance != null)
        {
            string playerID = PlayerPrefs.GetString("ActivePlayerID", "defaultPlayer");
            SaveManager.Instance.SavePlayerData(playerID);
            lastSaveTime = Time.time;
            
            // Show message if enabled
            if (showSaveMessage && uiManager != null)
            {
                uiManager.ShowNotification(saveMessage, messageDisplayTime);
            }
            else if (showSaveMessage)
            {
                Debug.Log(saveMessage);
            }
            
            // Invoke events
            onSave?.Invoke();
        }
        else
        {
            Debug.LogWarning("SaveTrigger - SaveManager not found. Cannot save game.");
        }
    }
}

/// <summary>
/// Basic UI manager interface - implement this in your actual UI system
/// </summary>
public class UIManager : MonoBehaviour
{
    public void ShowNotification(string message, float duration)
    {
        Debug.Log($"UI Notification: {message} (Duration: {duration}s)");
        // Implement actual UI notification system here
    }
} 