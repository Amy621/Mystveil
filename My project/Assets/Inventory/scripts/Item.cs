using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotTag { None, Head, Body, Wand, Accessory }
public enum ItemType {Herb, Potion, MonsterDrop, Armor}

[CreateAssetMenu(menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    public Sprite sprite;
    public SlotTag slotTag;
    public ItemType itemType;
    public bool stackable;
    public bool craftable = false;
    public string descripton;
    [Header("If the item can be crafted")]
    public Recipe recipe;

}