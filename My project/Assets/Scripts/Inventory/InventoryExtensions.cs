using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Extension methods for the Inventory class
/// </summary>
public static class InventoryExtensions
{
    /// <summary>
    /// Get the items array from the Inventory via reflection (since it's private)
    /// </summary>
    /// <param name="inventory">The inventory instance</param>
    /// <returns>The items array or null if not found</returns>
    public static Item[] GetItems(this Inventory inventory)
    {
        if (inventory == null)
            return null;
            
        // Use reflection to access the private items field
        FieldInfo itemsField = typeof(Inventory).GetField("items", 
            BindingFlags.NonPublic | BindingFlags.Instance);
            
        if (itemsField != null)
        {
            return itemsField.GetValue(inventory) as Item[];
        }
        
        return null;
    }
    
    /// <summary>
    /// Try to find an item by name in the inventory
    /// </summary>
    /// <param name="inventory">The inventory instance</param>
    /// <param name="itemName">The name of the item to find</param>
    /// <returns>The item or null if not found</returns>
    public static Item FindItemByName(this Inventory inventory, string itemName)
    {
        if (inventory == null || string.IsNullOrEmpty(itemName))
            return null;
            
        // First check the inventoryAmts dictionary
        foreach (var kvp in inventory.itemAmts)
        {
            if (kvp.Key.name == itemName)
                return kvp.Key;
        }
        
        // Then try the items array
        Item[] items = inventory.GetItems();
        if (items != null)
        {
            foreach (Item item in items)
            {
                if (item != null && item.name == itemName)
                    return item;
            }
        }
        
        return null;
    }
} 