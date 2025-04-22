using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem myItem { get; set; } //cur item in slot
    public SlotTag myTag;
    public Sprite defaultSprite;
    public Action OnItemClickedInBattle;

    
    public void OnPointerClick(PointerEventData eventData)
    {
        //on left click, place item in slot
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Inventory.Singleton.isIn1v1)
            {
                Inventory.Singleton.battle.useItem = myItem.myItem;
                Debug.Log("Inventory slot my item: " + myItem.myItem);
                Inventory.Singleton.removeItems(null, 1, this);
                Inventory.Singleton.view1v1();

                // Invoke the action to signal Battle to use the item
                Inventory.Singleton.battle.onUseItemRequested?.Invoke();
            }
            else
            {
                Debug.Log("clicked on slot");
                if(Input.GetKey(KeyCode.LeftShift)){
                    //shift click
                    Inventory.Singleton.Combine(this);
                }else{
                    if(myItem == null && Inventory.carriedItem != null) //place carried into empty slot
                        SetItem(Inventory.carriedItem);
                    else if(myItem != null)
                    Inventory.Singleton.SetCarriedItem(myItem); 
                }
            }            
        }
    }

    public void SetItem(InventoryItem item) //place "item" in slot
    {
        bool isEmpty = myItem == null;
        if(item != null && myTag != SlotTag.None && item.myItem.slotTag != myTag){ Debug.Log("cant place "+item+" here. this slot is "+myTag+" and you cant place "+item.myItem.slotTag);return; } //check if item is equippable in slot
        if(item != null) 
            Debug.Log("setting " + item + " in slot " + this);
        if(!isEmpty) //slot contains an item
        {
            Debug.Log("this slot already contains " + myItem);
            if(myItem.myItem == item.myItem && myItem.myItem.stackable) //if item is already in slot, stack
            {
                
                Debug.Log("stacking " + item.amount + " of " +item.myItem+ " in " +this + " which already has " + myItem.amount + " of " + myItem.myItem);
                myItem.amount = myItem.amount + item.amount;
                myItem.SetText();
                Debug.Log(this +" now has " + myItem.amount + " of " + myItem.myItem);
                if(myTag != SlotTag.None) //if slot is equippable, set item to be equiped
                {
                    changeImage();
                }
                return;
            }
        }

        if(myTag != SlotTag.None)
            changeImage();

        item.activeSlot.myItem = null; //remove item from previous slot
        
        //set cur slot
        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(this.transform);

        // Ensure the item is centered in the armor slot
        RectTransform itemRect = myItem.GetComponent<RectTransform>();
        itemRect.localPosition = Vector3.zero; // Center the item within the slot
        itemRect.localScale = Vector3.one; // Ensure scale is uniform


        myItem.canvasGroup.blocksRaycasts = true; //enable raycasting for item
        myItem.SetText();
        if(isEmpty)
            Inventory.carriedItem = null;

        if(myItem != null)
            Debug.Log(this + " now contains " + myItem.amount + " of " + myItem.myItem);
        else    
            Debug.Log(this + " is now empty");
        if(Inventory.carriedItem != null) 
            Debug.Log("now carrying " + Inventory.carriedItem.amount + " of " + Inventory.carriedItem); //debug log to show carried item
        //equip item if possible
        if(myTag != SlotTag.None) //if slot is equippable, set item to be equiped
        {
            changeImage();
            if(myItem != null)
                Debug.Log("equipping " + myItem.myItem + " to " + myTag);
            Inventory.Singleton.EquipEquipment(myTag, myItem);
        }
    }
    public void changeImage()
    {
        Image slotImage = GetComponentInChildren<Image>(); // this might find the wrong image if there are multiple
        if (myItem == null)
        {
            //set to background with icon
            slotImage.sprite = defaultSprite;
        }
        else
        {
            //set to empty background
            slotImage.sprite = Inventory.Singleton.inventorySlots[0].defaultSprite;
        }
    }
}


