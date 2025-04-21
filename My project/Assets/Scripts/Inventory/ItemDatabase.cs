using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Databases/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<GameItem> items = new List<GameItem>();
    
    // Get item by ID
    public GameItem GetItemByID(string itemID)
    {
        return items.Find(item => item.ItemID == itemID);
    }
    
    // Get all items
    public List<GameItem> GetAllItems()
    {
        return items;
    }
}

// GameItem class
[System.Serializable]
public class GameItem
{
    [SerializeField] private string itemID;
    [SerializeField] private string itemName;
    [SerializeField] private string description;
    [SerializeField] private bool isStackable = true;
    [SerializeField] private int maxDurability = 100;
    [SerializeField] private GameItemType itemType = GameItemType.Misc;
    
    // Default constructor for serialization
    public GameItem() { }
    
    // Constructor for creating items programmatically
    public GameItem(string id, string name, string desc, bool stackable, int durability, GameItemType type)
    {
        itemID = id;
        itemName = name;
        description = desc;
        isStackable = stackable;
        maxDurability = durability;
        itemType = type;
    }
    
    // Properties
    public string ItemID => itemID;
    public string Name => itemName;
    public string Description => description;
    public bool IsStackable => isStackable;
    public int MaxDurability => maxDurability;
    public GameItemType ItemType => itemType;
} 