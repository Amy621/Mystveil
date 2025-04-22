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
    public string description;
    [Header("If the item can be crafted")]
    public Recipe recipe;
    public MoveEffects effects;
    public MoveTarget target;
    
    public int hp = 0;
    public int spe = 0;
    public int spd = 0;
    public int def = 0;
    public int spa = 0;
    public int atk = 0;
    public int mana = 0;
    
}