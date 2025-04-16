using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using TMPro;

public class Crafting : MonoBehaviour
{
    public bool stay = false;
    public Item curSelected = null;
    public Dictionary<Item, bool> craftable = new Dictionary<Item, bool>(); //all items in the inventory and their amounts
    public bool isActive = false;
    public static Crafting instance;
    [SerializeField] GameObject recipefab;
    [SerializeField] GameObject craftingUI; 
    [SerializeField] Transform tr;
    [SerializeField] Item[] items;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        craftingUI.SetActive(isActive);
        foreach (Item item in items)
        {
            GameObject slot = Instantiate(recipefab, tr);
            RecipeSlot recipeSlot = slot.GetComponent<RecipeSlot>();
            recipeSlot.SetItem(item);
        }
        Inventory.Singleton.updateCount();
        categorizeCraftable();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)){
            isActive = !isActive;
            categorizeCraftable();
            craftingUI.SetActive(isActive);
            stay = false;
        }
    }
    

    
    public void categorizeCraftable(){
        foreach (Item item in items)
        {
            Recipe recipe = item.recipe;
            if(Inventory.Singleton.itemAmts.ContainsKey(recipe.ingredient1) && recipe.amount1 <= Inventory.Singleton.itemAmts[recipe.ingredient1])
            {
                if(Inventory.Singleton.itemAmts.ContainsKey(recipe.ingredient2) && recipe.amount2 <= Inventory.Singleton.itemAmts[recipe.ingredient2])
                {
                    if((recipe.amount3 == 0)|| (Inventory.Singleton.itemAmts.ContainsKey(recipe.ingredient3) && recipe.amount3 <= Inventory.Singleton.itemAmts[recipe.ingredient3]))
                    {
                        craftable[item] = true;
                    }else
                    {
                        craftable[item] = false;
                    }
                }else
                {
                    craftable[item] = false;
                }
            }else
            {
                craftable[item] = false;
            }
        }
    }
    public void Craft(Item item)
    {
        Recipe recipe = item.recipe;
        
        Inventory.Singleton.obj.SetActive(true);
        Inventory.Singleton.removeItems(recipe.ingredient1, recipe.amount1);
        Inventory.Singleton.removeItems(recipe.ingredient2, recipe.amount2);
        if(recipe.amount3 > 0)
            Inventory.Singleton.removeItems(recipe.ingredient3, recipe.amount3);
        Inventory.Singleton.SpawnInventoryItem(item);
        Inventory.Singleton.updateCount();
        Inventory.Singleton.obj.SetActive(false);
        categorizeCraftable();
    }
}

