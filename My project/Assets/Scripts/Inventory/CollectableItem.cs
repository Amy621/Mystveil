using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] private Item itemData;
    [SerializeField] private int quantity = 1;
    [SerializeField] private bool destroyOnPickup = true;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupEffect;
    
    private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && pickupSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool itemAdded = false;
            
            // Try to add to both inventory systems
            
            // First, try InventoryManager
            InventoryManager inventoryManager = FindInventoryManager();
            
            if (inventoryManager != null && itemData != null)
            {
                // Convert the Item to GameItem
                GameItem gameItem = ConvertItemToGameItem(itemData);
                
                if (gameItem != null)
                {
                    // Add item to inventory
                    itemAdded = inventoryManager.AddItem(gameItem, quantity);
                    if (itemAdded)
                    {
                        Debug.Log($"Added {quantity} {itemData.name} to InventoryManager");
                    }
                }
            }
            
            // If not added through InventoryManager, try the UI inventory directly
            if (!itemAdded)
            {
                Inventory uiInventory = FindObjectOfType<Inventory>();
                if (uiInventory != null && itemData != null)
                {
                    // Add to UI inventory directly
                    uiInventory.SpawnInventoryItem(itemData, quantity);
                    itemAdded = true;
                    Debug.Log($"Added {quantity} {itemData.name} to UI Inventory");
                }
            }
            
            if (itemAdded)
            {
                // Play pickup sound
                if (audioSource != null && pickupSound != null)
                {
                    audioSource.PlayOneShot(pickupSound);
                }
                
                // Play pickup effect
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }
                
                // If this is part of a quest, notify the quest system
                QuestTracker questTracker = QuestTracker.Instance;
                if (questTracker != null)
                {
                    // Get colliders to find possible quest items
                    Collider[] itemColliders = GetComponents<Collider>();
                    foreach (var collider in itemColliders)
                    {
                        questTracker.CompleteQuest(itemData.name, gameObject);
                    }
                }
                
                // Destroy the object if needed
                if (destroyOnPickup)
                {
                    // If we're playing a sound, wait until it's done before destroying
                    if (audioSource != null && audioSource.isPlaying)
                    {
                        StartCoroutine(DestroyAfterSound());
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    // Just disable the collider if we don't want to destroy the object
                    foreach (var collider in GetComponents<Collider>())
                    {
                        collider.enabled = false;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Failed to add {itemData.name} to any inventory system!");
            }
        }
    }
    
    private IEnumerator DestroyAfterSound()
    {
        // Disable the renderer and collider but keep the sound playing
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
        
        foreach (var collider in GetComponents<Collider>())
        {
            collider.enabled = false;
        }
        
        // Wait for the sound to finish playing
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        
        // Destroy the object
        Destroy(gameObject);
    }
    
    private InventoryManager FindInventoryManager()
    {
        // First try to find it directly on the player
        InventoryManager inventoryManager = GameObject.FindGameObjectWithTag("Player")?.GetComponent<InventoryManager>();
        
        // If not found, look for it anywhere in the scene
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
        
        return inventoryManager;
    }
    
    private GameItem ConvertItemToGameItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("No item data assigned to CollectableItem!");
            return null;
        }
        
        // Create a new GameItem using the constructor
        // Use the ScriptableObject's name property both as ID and display name
        return new GameItem(
            id: item.name,             // Item ID 
            name: item.name,           // Display name (ScriptableObject name)
            desc: item.description,    // Description
            stackable: item.stackable, // Is stackable
            durability: 100,           // Default max durability
            type: ConvertItemType(item.itemType) // Item type
        );
    }
    
    private GameItemType ConvertItemType(ItemType itemType)
    {
        // Convert from the old Item.ItemType enum to the new GameItem.GameItemType enum
        switch (itemType)
        {
            case ItemType.Herb:
                return GameItemType.Material;
            case ItemType.Potion:
                return GameItemType.Consumable;
            case ItemType.MonsterDrop:
                return GameItemType.Material;
            case ItemType.Armor:
                return GameItemType.Armor;
            default:
                return GameItemType.Misc;
        }
    }
} 