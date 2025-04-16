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

// Placeholder Item class
[System.Serializable]
public class GameItem
{
    [SerializeField] private string itemID;
    [SerializeField] private string itemName;
    [SerializeField] private bool isStackable = true;
    [SerializeField] private int maxDurability = 100;
    
    // Properties
    public string ItemID => itemID;
    public string ItemName => itemName;
    public bool IsStackable => isStackable;
    public int MaxDurability => maxDurability;
} 