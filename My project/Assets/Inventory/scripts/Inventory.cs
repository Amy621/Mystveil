using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting.Dependencies.Sqlite;
//using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    //public Dictionary<InventoryItem, List<InventorySlot>> itempos; //lists all posititions of items
    public static Inventory Singleton; //only one instance of the inventory can exist
    public static InventoryItem carriedItem; //item currently being dragged in inventory
    [SerializeField] InventorySlot[] inventorySlots; //all slots in the inventory/armor/hotbar

    [SerializeField] Transform curCarryingTransform; //no clue, transform is a ui thing idk
    [SerializeField] InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] Item[] items; //all items in the game

    [Header("Debug")]
    [SerializeField] Button giveItemButton; //button to give item for testing

    public GameObject obj;

    private bool isActive = false;

    void Awake()
    {
        //itempos = new Dictionary<InventoryItem, List<InventorySlot>>(); //initialize item positions
        Singleton = this; //init single inventory
        giveItemButton.onClick.AddListener(delegate { SpawnInventoryItem(); });
        obj.SetActive(isActive);
    }

    //spawn an item in the inventory for testing
    public void SpawnInventoryItem(Item item = null, int amt = 1)
    {
        Item itm = item;
        if (itm == null)
        {
            int random = Random.Range(0, items.Length);
            itm = items[random]; //get a random item from the list
        }
        int firstEmpty = -1;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].myItem != null && inventorySlots[i].myItem.myItem == itm && inventorySlots[i].myTag == SlotTag.None)
            {
                //stack in found slot
                inventorySlots[i].myItem.amount += amt;
                inventorySlots[i].myItem.SetText();
                return;
            }
            else if (firstEmpty == -1 && inventorySlots[i].myItem == null)
            {
                firstEmpty = i; //find first empty slot
            }
        }
        Instantiate(itemPrefab, inventorySlots[firstEmpty].transform).Initialize(itm, inventorySlots[firstEmpty]); //create item in slot
    }

    void Update() //item follow cursor
    {
        if (carriedItem != null)
        {
            carriedItem.transform.position = Input.mousePosition;
        }

        if(Input.GetKeyDown(KeyCode.I)){
            Debug.Log("EEEEEEEEEEEEE");

            if(carriedItem != null){
                for(int i = 0; i < inventorySlots.Length; i++){
                    if(inventorySlots[i].myItem == null){
                        inventorySlots[i].SetItem(carriedItem);
                        carriedItem = null;
                        break;
                    }
                }
            }

            isActive = !isActive;
            obj.SetActive(isActive);
        }

    }

    //set carried item to be the item in the slot
    public void SetCarriedItem(InventoryItem item) //"item" is the item in the slot to be carried
    {

        Debug.Log("check hello1");
        if(carriedItem != null)
            Debug.Log("what");
        if (carriedItem != null)
        {
            Debug.Log(item.activeSlot);
            if (item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag != carriedItem.myItem.slotTag)
            { Debug.Log(carriedItem + " cant be placed in " + item.activeSlot.myTag); return; } //cur carried item cant be placed in equipment slot

            Debug.Log("placing " + carriedItem.amount + " of " + carriedItem + " in " + item.activeSlot); //debug log to show where item is being placed

            item.activeSlot.SetItem(carriedItem); //place carried item in slot ///////////

            if (carriedItem != null && carriedItem.myItem == item.myItem && carriedItem.myItem.stackable)
            { //stack
                carriedItem.activeSlot.myItem = null;
                carriedItem.transform.SetParent(null);
                Destroy(carriedItem);
                carriedItem = null;
                Debug.Log("stacked");
                return;
            }

        }
        else
        {
            Debug.Log("Hello2");
            //item.activeSlot.myItem = null; //remove item from previous slot
        }
        Debug.Log("debug check3.");
        
        if (item == null) 
        {
            Debug.Log("D4");
            carriedItem = null;
            Debug.Log("carried item is null");
        }
        else //slot not empty
        {
            carriedItem = item;
            Debug.Log("carried item not null");
            carriedItem.activeSlot.myItem = null; 
            carriedItem.canvasGroup.blocksRaycasts = false; //disable raycasting for carried item
            carriedItem.transform.SetParent(curCarryingTransform); //set carried item to follow cursor
            Debug.Log("now carrying " + carriedItem.amount + " of " + carriedItem.myItem); //debug log to show carried item
        }
    }


    public void EquipEquipment(SlotTag tagm, InventoryItem item = null)
    {



        //apply stats
    }


    //on shift click, combine items in slot with like items in inv
    public void Combine(InventorySlot slot){
        if(slot == null || slot.myItem == null || slot.myItem.myItem.stackable == false || slot.myTag != SlotTag.None) return; //cant combine empty or non-stackable items
        Item item = slot.myItem.myItem;
        int firstEmpty = -1;
        for(int i = 0; i < inventorySlots.Length; i++){
            if(inventorySlots[i].myItem != null && inventorySlots[i].myItem.myItem == item && inventorySlots[i] != slot){
                //stack in found slot
                inventorySlots[i].myItem.amount += slot.myItem.amount;
                inventorySlots[i].myItem.SetText();
                slot.myItem.transform.SetParent(null);
                Destroy(slot.myItem); 
                slot.myItem = null;
                return;
            }else if(firstEmpty == -1 &&inventorySlots[i].myItem == null){
                firstEmpty = i;
            }
        }
        if(firstEmpty == -1) return;
        InventorySlot newSlot = inventorySlots[firstEmpty];
        newSlot.myItem = slot.myItem;
        newSlot.myItem.activeSlot = newSlot;
        newSlot.myItem.transform.SetParent(newSlot.transform);
        newSlot.myItem.SetText();
        //slot.myItem.transform.SetParent(null);
        //Destroy(slot.myItem);
        slot.myItem = null;
    }

}
