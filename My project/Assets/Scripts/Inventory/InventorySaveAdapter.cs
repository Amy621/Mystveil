using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryManager))]
public class InventorySaveAdapter : MonoBehaviour
{
    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
    }

    private void OnEnable()
    {
        // Register with the save system events
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.OnSave += SaveInventory;
            SimpleSaveSystem.Instance.OnLoad += LoadInventory;
        }
    }

    private void OnDisable()
    {
        // Unregister from the save system events
        if (SimpleSaveSystem.Instance != null)
        {
            SimpleSaveSystem.Instance.OnSave -= SaveInventory;
            SimpleSaveSystem.Instance.OnLoad -= LoadInventory;
        }
    }

    private void SaveInventory(SimpleSaveData saveData)
    {
        if (inventoryManager != null)
        {
            // Get serializable inventory data
            List<SerializableItem> serializableItems = inventoryManager.GetSerializableInventory();
            
            // Convert to arrays for serialization
            string[] itemIDs = new string[serializableItems.Count];
            int[] quantities = new int[serializableItems.Count];
            int[] durabilities = new int[serializableItems.Count];
            
            for (int i = 0; i < serializableItems.Count; i++)
            {
                SerializableItem item = serializableItems[i];
                itemIDs[i] = item.itemID;
                quantities[i] = item.quantity;
                durabilities[i] = item.durability;
                
                // Save enchantments if any
                if (item.enchantments != null && item.enchantments.Count > 0)
                {
                    saveData.SetStringArray("item_enchantments_" + item.itemID, item.enchantments.ToArray());
                }
            }
            
            // Save arrays to save data
            saveData.SetStringArray("inventory_itemIDs", itemIDs);
            
            // Convert int arrays to string arrays for storage
            string[] quantityStrings = new string[quantities.Length];
            string[] durabilityStrings = new string[durabilities.Length];
            
            for (int i = 0; i < quantities.Length; i++)
            {
                quantityStrings[i] = quantities[i].ToString();
                durabilityStrings[i] = durabilities[i].ToString();
            }
            
            saveData.SetStringArray("inventory_quantities", quantityStrings);
            saveData.SetStringArray("inventory_durabilities", durabilityStrings);
            
            Debug.Log($"[InventorySaveAdapter] Saved {serializableItems.Count} inventory items");
        }
    }

    private void LoadInventory(SimpleSaveData saveData)
    {
        if (inventoryManager != null)
        {
            // Get saved arrays
            string[] itemIDs = saveData.GetStringArray("inventory_itemIDs");
            string[] quantityStrings = saveData.GetStringArray("inventory_quantities");
            string[] durabilityStrings = saveData.GetStringArray("inventory_durabilities");
            
            if (itemIDs == null || quantityStrings == null || durabilityStrings == null)
            {
                Debug.Log("[InventorySaveAdapter] No inventory data found in save file");
                return;
            }
            
            // Convert string arrays back to int arrays
            int[] quantities = new int[quantityStrings.Length];
            int[] durabilities = new int[durabilityStrings.Length];
            
            for (int i = 0; i < quantityStrings.Length; i++)
            {
                int.TryParse(quantityStrings[i], out quantities[i]);
                int.TryParse(durabilityStrings[i], out durabilities[i]);
            }
            
            // Create serializable items
            List<SerializableItem> serializableItems = new List<SerializableItem>();
            
            for (int i = 0; i < itemIDs.Length; i++)
            {
                // Get enchantments if any
                string[] enchantmentArray = saveData.GetStringArray("item_enchantments_" + itemIDs[i]);
                List<string> enchantments = enchantmentArray != null
                    ? new List<string>(enchantmentArray)
                    : new List<string>();
                
                SerializableItem item = new SerializableItem(
                    itemIDs[i],
                    i < quantities.Length ? quantities[i] : 1,
                    i < durabilities.Length ? durabilities[i] : 100,
                    enchantments
                );
                
                serializableItems.Add(item);
            }
            
            // Load inventory
            inventoryManager.LoadInventory(serializableItems);
            
            Debug.Log($"[InventorySaveAdapter] Loaded {serializableItems.Count} inventory items");
        }
    }
} 