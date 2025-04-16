using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecipeDisplay : MonoBehaviour
{
    Item item = null;
    bool isActive = false;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI[] ingredientTexts;
    [SerializeField] TextMeshProUGUI[] ingredientAmts;
    [SerializeField] Image itemImage;
    [SerializeField] Image[] ingredientImages;
    [SerializeField] GameObject[] ingredients;

    // Start is called before the first frame update
    void Start()
    {
        hideDisplay();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void hideDisplay(){
        for(int i = 0; i < ingredients.Length; i++)
        {
            ingredients[i].SetActive(false);
        }
        itemImage.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
    }
    public void UpdateRecipeDisplay(Item newItem)
    {
        if(newItem == null) return;
        item = newItem;
        Recipe recipe = item.recipe;
        titleText.text = item.name;
        descriptionText.text = item.description;
        Debug.Log("Description: " + item.description);
        itemImage.sprite = item.sprite;
        activate();
        // Set the ingredient texts
        ingredientTexts[0].text = recipe.ingredient1.name;
        ingredientImages[0].sprite = recipe.ingredient1.sprite;
        string amt1;
        if(!Inventory.Singleton.itemAmts.ContainsKey(recipe.ingredient1))
            amt1 = "0";
        else
            amt1 = Inventory.Singleton.itemAmts[recipe.ingredient1].ToString();
        ingredientAmts[0].text = amt1 + "/" + recipe.amount1.ToString();
        

        ingredientTexts[1].text = recipe.ingredient2.name;
        ingredientImages[1].sprite = recipe.ingredient2.sprite;
        string amt2;
        if(!Inventory.Singleton.itemAmts.ContainsKey(recipe.ingredient2))
            amt2 = "0";
        else
            amt2 = Inventory.Singleton.itemAmts[recipe.ingredient2].ToString();
        ingredientAmts[1].text = amt2 + "/" + recipe.amount2.ToString();

        if(recipe.amount3 > 0)
        {
            ingredientTexts[2].text = recipe.ingredient3.name;
            ingredientImages[2].sprite = recipe.ingredient3.sprite;
            string amt3;
            if(!Inventory.Singleton.itemAmts.ContainsKey(recipe.ingredient3))
                amt3 = "0";
            else
                amt3 = Inventory.Singleton.itemAmts[recipe.ingredient3].ToString();
            ingredientAmts[2].text = amt3 + "/" + recipe.amount3.ToString();
            //ingredientTexts[2].gameObject.SetActive(true);
            //ingredientImages[2].gameObject.SetActive(true);
            //ingredientAmts[2].gameObject.SetActive(true);
        }
        else
        {
            ingredientTexts[2].text = null;
            ingredientImages[2].sprite = null;
            ingredientAmts[2].text = null;
            ingredients[2].SetActive(false);
            //ingredientTexts[2].gameObject.SetActive(false);
            //ingredientImages[2].gameObject.SetActive(false);
            //ingredientAmts[2].gameObject.SetActive(false);
        }
        
    }
    public void activate(){
        titleText.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);
        itemImage.gameObject.SetActive(true);
        int numIngredients = 2;
        if(item.recipe.amount3 > 0)
            numIngredients = 3;

        for (int i = 0; i < numIngredients; i++)
        {
            ingredients[i].SetActive(true);
        }
        isActive = true;
    }

    public void CraftItem()
{
    if (item == null || item.recipe == null) return;

    Recipe recipe = item.recipe;
    // Check if player has enough of the ingredients
    if (Crafting.instance.craftable[item] == true)
    {
        
        Crafting.instance.Craft(item); // Give the player the crafted item
        Debug.Log("Crafted: " + item.name);
        UpdateRecipeDisplay(item);
    }
    else
    {
        Debug.Log("Not enough ingredients to craft: " + item.name);
        Debug.Log("Have: " + Inventory.Singleton.itemAmts[recipe.ingredient1] + " of " + recipe.ingredient1.name + ", " + Inventory.Singleton.itemAmts[recipe.ingredient1] + " of " + recipe.ingredient2.name);
        if (recipe.amount3 > 0)
        {
            Debug.Log("Need: " + recipe.amount3 + " of " + recipe.ingredient3.name);
        }
        
    }
}
}
