using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(menuName = "ScriptableObjects/Item")]
public class Recipe : ScriptableObject
{
    public Sprite sprite;
    public SlotTag slotTag;
    public bool stackable;


    [Header("If the item can be equipped")]
    public GameObject prefab;
}
