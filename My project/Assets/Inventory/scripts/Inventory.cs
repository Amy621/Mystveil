using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    //public Dictionary<InventoryItem, List<InventorySlot>> itempos; //lists all posititions of items
    public static Inventory Singleton; //only one instance of the inventory can exist
    public static InventoryItem carriedItem; //item currently being dragged in inventory
    [SerializeField] InventorySlot[] inventorySlots; //slots for equippable items

    [SerializeField] Transform curCarryingTransform; //no clue, transform is a ui thing idk
    [SerializeField] InventoryItem itemPrefab; 

    [Header("Item List")]
    [SerializeField] Item[] items; //all items in the game

    [Header("Debug")]
    [SerializeField] Button giveItemButton; //button to give item for testing



    void Awake()
    {
        //itempos = new Dictionary<InventoryItem, List<InventorySlot>>(); //initialize item positions
       Singleton = this; //init single inventory
       giveItemButton.onClick.AddListener(delegate{SpawnInventoryItem();});

    }

    //spawn an item in the inventory for testing
    public void SpawnInventoryItem(Item item = null)
    {
        Item itm = item;
        if(itm == null){
            int random = Random.Range(0, items.Length);
            itm = items[random]; //get a random item from the list
        }

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].myItem == null) //find an empty slot
            {
                Debug.Log("placing " + itm + " in slot " + i);
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(itm, inventorySlots[i]); //create item in slot
                break;
            }
        }
    }

    void Update() //item follow cursor
    {
        if(carriedItem != null)
        {
            carriedItem.transform.position = Input.mousePosition;
        }
    }

    //set carried item to be the item in the slot
    public void SetCarriedItem(InventoryItem item) //"item" is the item in the slot to be carried
    {

        
        if(carriedItem != null)
        {
            if(item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag != carriedItem.myItem.itemTag) 
            { Debug.Log(carriedItem + " cant be placed in " + item.activeSlot.myTag);return;} //cur carried item cant be placed in equipment slot
            Debug.Log("placing " + carriedItem.amount + " of " + carriedItem +" in " + item.activeSlot); //debug log to show where item is being placed
            
            item.activeSlot.SetItem(carriedItem); //place carried item in slot ///////////

            if(carriedItem != null && carriedItem.myItem == item.myItem){ //stack
                carriedItem.canvasGroup.blocksRaycasts = false;
                carriedItem.transform.SetParent(null);
                Destroy(carriedItem); 
                carriedItem = null;
                Debug.Log("stacked");
                return;
            }
            
        }
        else
        {
            item.activeSlot.myItem = null; //remove item from previous slot
        }
        carriedItem = item;
        if(item == null) Debug.Log(" item is null"); 
        else Debug.Log("item not null");
        carriedItem.canvasGroup.blocksRaycasts = false; //disable raycasting for carried item
        item.transform.SetParent(curCarryingTransform); //set carried item to follow cursor
        if(carriedItem != null)
            Debug.Log("now carrying " + carriedItem.amount + " of " + carriedItem.myItem); //debug log to show carried item
        /*
        
        if(carriedItem != null) //switch if carrying an item already
        {

            if(item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag == carriedItem.myItem.itemTag) 
            { //if the item is equippable, check if it can be placed in the slot

                //EquipEquipment(item.activeSlot.myTag, carriedItem); //equip carried item to the slot
                //carriedItem.SetActiveSlot(item.activeSlot); //set carried item to the new slot
                carriedItem = item; //carry removed equipment
            }
            else if(item.activeSlot.myTag == SlotTag.None) 
            {
                carriedItem.transform.SetParent(item.activeSlot.transform); 
            }
            item.activeSlot.SetItem(carriedItem); //place carried item in the slot
        }

        //unequip item if it was in an equipment slot
        if(item.activeSlot.myTag != SlotTag.None)
        {
            EquipEquipment(item.activeSlot.myTag, null);
        }

        carriedItem = item;
        carriedItem.canvasGroup.blocksRaycasts = false; 
        item.transform.SetParent(curCarryingTransform); //set carried item to follow cursor
        */
    }

    
    public void EquipEquipment(SlotTag tagm, InventoryItem item = null){
        

        
        //apply stats
    }
}