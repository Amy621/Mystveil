using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameInventoryItem
{
    public GameItem itemData;
    public int quantity;
    public int durability;
    public List<string> enchantments;
    
    public GameInventoryItem(GameItem item, int qty, int dur, List<string> ench)
    {
        itemData = item;
        quantity = qty;
        durability = dur;
        enchantments = ench;
    }
} 