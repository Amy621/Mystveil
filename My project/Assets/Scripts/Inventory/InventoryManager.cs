using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
    [SerializeField] private int maxInventorySlots = 30;
    
    private Dictionary<string, GameInventoryItem> inventory = new Dictionary<string, GameInventoryItem>();
    
    public delegate void InventoryChangedEvent();
    public event InventoryChangedEvent OnInventoryChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Add an item to inventory
    public bool AddItem(GameItem item, int quantity = 1)
    {
        if (quantity <= 0)
            return false;
            
        string itemID = item.ItemID;
        
        // Check if inventory is full
        if (!inventory.ContainsKey(itemID) && inventory.Count >= maxInventorySlots)
        {
            Debug.Log("Inventory is full");
            return false;
        }
        
        // Check if the item exists in inventory
        if (inventory.ContainsKey(itemID))
        {
            // Stackable item - increase quantity
            if (item.IsStackable)
            {
                inventory[itemID].quantity += quantity;
            }
            // Non-stackable - add as new item with unique ID
            else
            {
                string uniqueItemID = itemID + "_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                inventory.Add(uniqueItemID, new GameInventoryItem(item, quantity, item.MaxDurability, new List<string>()));
            }
        }
        else
        {
            // Add new item to inventory
            inventory.Add(itemID, new GameInventoryItem(item, quantity, item.MaxDurability, new List<string>()));
        }
        
        // Trigger event
        OnInventoryChanged?.Invoke();
        
        return true;
    }
    
    // Remove item from inventory
    public bool RemoveItem(string itemID, int quantity = 1)
    {
        if (!inventory.ContainsKey(itemID) || quantity <= 0)
            return false;
            
        inventory[itemID].quantity -= quantity;
        
        // Remove the item if quantity reaches 0
        if (inventory[itemID].quantity <= 0)
        {
            inventory.Remove(itemID);
        }
        
        // Trigger event
        OnInventoryChanged?.Invoke();
        
        return true;
    }
    
    // Get item from inventory
    public GameInventoryItem GetItem(string itemID)
    {
        if (inventory.ContainsKey(itemID))
            return inventory[itemID];
            
        return null;
    }
    
    // Check if player has the item
    public bool HasItem(string itemID, int quantity = 1)
    {
        if (!inventory.ContainsKey(itemID))
            return false;
            
        return inventory[itemID].quantity >= quantity;
    }
    
    // Get all items in inventory
    public List<GameInventoryItem> GetAllItems()
    {
        return inventory.Values.ToList();
    }
    
    // Get serializable inventory data for saving
    public List<SerializableItem> GetSerializableInventory()
    {
        List<SerializableItem> serializableItems = new List<SerializableItem>();
        
        foreach (var kvp in inventory)
        {
            GameInventoryItem item = kvp.Value;
            serializableItems.Add(new SerializableItem(
                kvp.Key,
                item.quantity,
                item.durability,
                item.enchantments
            ));
        }
        
        return serializableItems;
    }
    
    // Load inventory from serializable data
    public void LoadInventory(List<SerializableItem> serializableItems)
    {
        // Clear current inventory
        inventory.Clear();
        
        // Get ItemDatabase reference
        ItemDatabase itemDB = FindObjectOfType<ItemDatabase>();
        
        if (itemDB != null)
        {
            foreach (SerializableItem sItem in serializableItems)
            {
                // Get base item data from database
                string baseItemID = sItem.itemID;
                
                // Handle unique item IDs
                if (sItem.itemID.Contains("_"))
                {
                    baseItemID = sItem.itemID.Split('_')[0];
                }
                
                GameItem itemData = itemDB.GetItemByID(baseItemID);
                
                if (itemData != null)
                {
                    GameInventoryItem invItem = new GameInventoryItem(
                        itemData,
                        sItem.quantity,
                        sItem.durability,
                        sItem.enchantments
                    );
                    
                    inventory.Add(sItem.itemID, invItem);
                }
                else
                {
                    Debug.LogWarning($"Item with ID {sItem.itemID} not found in database during load");
                }
            }
        }
        else
        {
            Debug.LogError("ItemDatabase not found when loading inventory!");
        }
        
        // Trigger event
        OnInventoryChanged?.Invoke();
    }
    
    // Reset inventory to default state
    public void ResetToDefault()
    {
        inventory.Clear();
        
        // Here you could add starting items for a new player
        ItemDatabase itemDB = FindObjectOfType<ItemDatabase>();
        if (itemDB != null)
        {
            // Add starting sword
            GameItem startingSword = itemDB.GetItemByID("weapon_sword_basic");
            if (startingSword != null)
            {
                AddItem(startingSword);
            }
            
            // Add starting health potions
            GameItem healthPotion = itemDB.GetItemByID("potion_health_small");
            if (healthPotion != null)
            {
                AddItem(healthPotion, 3);
            }
        }
        
        // Trigger event
        OnInventoryChanged?.Invoke();
    }
} 