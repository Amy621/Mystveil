using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class RecipeSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    RecipeDisplay recipeDisplay;
   [Serialize]public Item item;
   
    public Image image;
    void Awake(){
        Transform child = transform.Find("Recipe Image");
        if (child != null)
        {
            recipeDisplay = FindObjectOfType<RecipeDisplay>();
            image = child.GetComponent<Image>();
        }
    }
   public void OnPointerClick(PointerEventData eventData)
    {
        //on left click, place item in slot
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Crafting.instance.stay = true;
            Crafting.instance.curSelected = item;
            recipeDisplay.UpdateRecipeDisplay(item);
        }
    }

    public void SetItem(Item newItem)
    {
        item = newItem;
        image.sprite = item.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            recipeDisplay.UpdateRecipeDisplay(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!Crafting.instance.stay)
            recipeDisplay.hideDisplay();
        else
            recipeDisplay.UpdateRecipeDisplay(Crafting.instance.curSelected);
    }
}
