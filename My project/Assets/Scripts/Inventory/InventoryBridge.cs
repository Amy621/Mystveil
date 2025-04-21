using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class serves as a bridge between the InventoryManager (for saving/collection) 
/// and the UI-based Inventory system.
/// </summary>
public class InventoryBridge : MonoBehaviour
{
    [SerializeField] private Inventory inventoryUI;
    
    private InventoryManager inventoryManager;
    private ItemDatabase itemDatabase;
    
    private void Awake()
    {
        // Try to get or create an InventoryManager
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            GameObject inventoryManagerObj = new GameObject("InventoryManager");
            inventoryManager = inventoryManagerObj.AddComponent<InventoryManager>();
            inventoryManagerObj.AddComponent<InventorySaveAdapter>();
        }
        
        // Try to find the inventory UI
        if (inventoryUI == null)
        {
            inventoryUI = FindObjectOfType<Inventory>();
        }
        
        // Get the ItemDatabase
        itemDatabase = FindObjectOfType<ItemDatabase>();
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase not found in scene!");
        }
    }
    
    private void OnEnable()
    {
        // Subscribe to inventory change events
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged += SyncToUIInventory;
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from inventory change events
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged -= SyncToUIInventory;
        }
    }
    
    /// <summary>
    /// Synchronize the InventoryManager's contents to the UI Inventory
    /// </summary>
    public void SyncToUIInventory()
    {
        if (inventoryUI == null || inventoryManager == null)
            return;
            
        // Clear inventory UI first
        ClearUIInventory();
        
        // Get all items from InventoryManager
        List<GameInventoryItem> items = inventoryManager.GetAllItems();
        
        // Add each item to the UI inventory
        foreach (GameInventoryItem gameItem in items)
        {
            // Convert GameItem to Item
            Item uiItem = FindUIItemEquivalent(gameItem.itemData);
            
            if (uiItem != null)
            {
                inventoryUI.SpawnInventoryItem(uiItem, gameItem.quantity);
            }
        }
    }
    
    /// <summary>
    /// Clear all items from the UI inventory
    /// </summary>
    private void ClearUIInventory()
    {
        if (inventoryUI == null)
            return;
            
        foreach (InventorySlot slot in inventoryUI.inventorySlots)
        {
            if (slot.myItem != null)
            {
                Destroy(slot.myItem.gameObject);
                slot.myItem = null;
            }
        }
        
        // Also clear the carried item if there is one
        if (Inventory.carriedItem != null)
        {
            Destroy(Inventory.carriedItem.gameObject);
            Inventory.carriedItem = null;
        }
        
        inventoryUI.itemAmts.Clear();
    }
    
    /// <summary>
    /// Find the Item equivalent in the UI system for a GameItem from InventoryManager
    /// </summary>
    private Item FindUIItemEquivalent(GameItem gameItem)
    {
        if (inventoryUI == null)
            return null;
            
        // Use our extension method to find the item by name
        Item foundItem = inventoryUI.FindItemByName(gameItem.ItemID);
        if (foundItem != null)
        {
            return foundItem;
        }
        
        // If not found, try loading from Resources
        string itemPath = "Items/" + gameItem.ItemID;
        Item loadedItem = Resources.Load<Item>(itemPath);
        if (loadedItem != null)
        {
            return loadedItem;
        }
        
        Debug.LogWarning($"Could not find UI Item equivalent for {gameItem.Name} (ID: {gameItem.ItemID})");
        return null;
    }
    
    /// <summary>
    /// Add the InventoryManager component to the UI Inventory GameObject
    /// </summary>
    public static void AddInventoryManagerToUI()
    {
        Inventory inventoryUI = FindObjectOfType<Inventory>();
        if (inventoryUI != null)
        {
            GameObject inventoryObj = inventoryUI.gameObject;
            
            // Add InventoryManager if it doesn't exist
            if (inventoryObj.GetComponent<InventoryManager>() == null)
            {
                inventoryObj.AddComponent<InventoryManager>();
            }
            
            // Add InventorySaveAdapter if it doesn't exist
            if (inventoryObj.GetComponent<InventorySaveAdapter>() == null)
            {
                inventoryObj.AddComponent<InventorySaveAdapter>();
            }
            
            // Add this bridge component
            if (inventoryObj.GetComponent<InventoryBridge>() == null)
            {
                InventoryBridge bridge = inventoryObj.AddComponent<InventoryBridge>();
                bridge.inventoryUI = inventoryUI;
            }
            
            Debug.Log("Added InventoryManager to UI Inventory GameObject");
        }
        else
        {
            Debug.LogError("No Inventory UI found in scene!");
        }
    }
} 