using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem myItem { get; set; } //cur item in slot
    public SlotTag myTag;

    
    public void OnPointerClick(PointerEventData eventData)
    {
        //on left click, place item in slot
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("clicked on slot");
            if(myItem == null && Inventory.carriedItem != null) //place carried into empty slot
                SetItem(Inventory.carriedItem);
            else if(myItem != null)
                Inventory.Singleton.SetCarriedItem(myItem); 
            /*
            if(Inventory.carriedItem == null) return; //no item to place
            //non-equippable items cant be in equipment slot
            if(myTag != SlotTag.None && Inventory.carriedItem.myItem.itemTag != myTag) return;
            if(myItem == null || myItem.myItem == Inventory.carriedItem.myItem){
                SetItem(Inventory.carriedItem);
                myItem.amount++;
            }
            */
        }
    }

    public void SetItem(InventoryItem item) //assumes slot is empty
    {
        if(item != null) 
            Debug.Log("setting " + item + " in slot " + this);
        if(myItem != null)
        {
            Debug.Log("this slot already contains " + myItem);
            if(myTag != SlotTag.None && item.myItem.itemTag != myTag){ Debug.Log("cant place "+item+" here. this slot is "+myTag+" and you cant place "+item.myItem.itemTag);return; }//check if item is equippable in slot
            if(myItem.myItem == item.myItem && myItem.myItem.stackable) //if item is already in slot, stack
            {
                
                Debug.Log("stacking " + item.amount + " of " +item.myItem+ " in " +this + " which already has " + myItem.amount + " of " + myItem.myItem);
                myItem.amount += item.amount;
                //Inventory.carriedItem = null; //clear carried item
                //Destroy(Inventory.carriedItem); 
                Debug.Log(this +" now has " + myItem.amount + " of " + myItem.myItem);
                return;
            }
        }


        Inventory.carriedItem = null;

        item.activeSlot.myItem = null; //remove item from previous slot
        
        //set cur slot
        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);
        myItem.canvasGroup.blocksRaycasts = true; //enable raycasting for item

        if(myItem != null)
            Debug.Log(this + " now contains " + myItem.amount + " of " + myItem.myItem);
        else    
            Debug.Log(this + " is now empty");
        myItem.SetText(); //update text to show amount of item in slot
        if(Inventory.carriedItem != null) 
            Debug.Log("now carrying " + Inventory.carriedItem.amount + " of " + Inventory.carriedItem); //debug log to show carried item
        //else
            //Debug.Log("no item is being carried"); 
        //equip item if possible
        if(myTag != SlotTag.None) //if slot is equippable, set item to be equiped
        {
            if(myItem != null)
                Debug.Log("equipping " + myItem.myItem + " to " + myTag);
            Inventory.Singleton.EquipEquipment(myTag, myItem);
        }
    }

}
