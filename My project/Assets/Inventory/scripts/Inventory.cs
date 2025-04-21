using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    public Dictionary<Item, int> itemAmts = new Dictionary<Item, int>(); //all items in the inventory and their amounts
    public static Inventory Singleton; //only one instance of the inventory can exist
    public static InventoryItem carriedItem; //item currently being dragged in inventory
    [SerializeField] public InventorySlot[] inventorySlots; //all slots in the inventory/armor/hotbar

    [SerializeField] Transform curCarryingTransform; //no clue, transform is a ui thing idk
    [SerializeField] InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] Item[] items; //all items in the game

    [Header("Debug")]
    [SerializeField] Button giveItemButton; //button to give item for testing

    public GameObject obj;
    public Sprite highlightSprite;
    public bool isActive = false;

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
                itemAmts[itm] += amt;
                return;
            }
            else if (firstEmpty == -1 && inventorySlots[i].myItem == null)
            {
                firstEmpty = i; //find first empty slot
            }
        }
        itemAmts[itm] = amt;
        Instantiate(itemPrefab, inventorySlots[firstEmpty].transform).Initialize(itm, inventorySlots[firstEmpty]); //create item in slot
        if(!isActive)
            obj.SetActive(false);
    }

    void Update() //item follow cursor
    {
        if (carriedItem != null)
        {
            carriedItem.transform.position = Input.mousePosition;
        }

        if(Input.GetKeyDown(KeyCode.I)){
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

    public int getSlotNum(){
        return inventorySlots.Length;
    }
    //set carried item to be the item in the slot
    public void SetCarriedItem(InventoryItem item) //"item" is the item in the slot to be carried
    {
        if(item != null && item.activeSlot.myTag != SlotTag.None)
            item.amountText.gameObject.SetActive(true); 
        if (carriedItem != null)
        {
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
        
        if (item == null) 
        {
            carriedItem = null;
            Debug.Log("carried item is null");
        }
        else //slot not empty
        {
            carriedItem = item;
            carriedItem.activeSlot.myItem = null; 
            carriedItem.canvasGroup.blocksRaycasts = false; //disable raycasting for carried item
            carriedItem.transform.SetParent(curCarryingTransform); //set carried item to follow cursor
            if(item.activeSlot != null && item.activeSlot.myTag != SlotTag.None)
                item.activeSlot.changeImage();
            Debug.Log("now carrying " + carriedItem.amount + " of " + carriedItem.myItem); //debug log to show carried item
        }
    }


    public void EquipEquipment(SlotTag tagm, InventoryItem item = null)
    {
        //null item means take it off
        if(item == null){
            //remove stat
            //activate text
            
        }
        else{
            //add stat
            //deactivate text
            item.amountText.gameObject.SetActive(false);
        }

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

    public void updateCount(){
        itemAmts.Clear();
        for(int i = 0; i < inventorySlots.Length; i++){
            if(inventorySlots[i].myItem != null){
                if(itemAmts.ContainsKey(inventorySlots[i].myItem.myItem)){
                    itemAmts[inventorySlots[i].myItem.myItem] += inventorySlots[i].myItem.amount;
                }else{
                    itemAmts.Add(inventorySlots[i].myItem.myItem, inventorySlots[i].myItem.amount);
                }
            }
        }
    }
    
    public void removeItems(Item item = null, int numRemove = -1, int slotNum = -1){
        if(item && (!itemAmts.ContainsKey(item) || itemAmts[item] < numRemove)){
            Debug.Log("not enough items to remove");
            return;
        }
        
        if(item == null){ //find item(s) to remove
            List<Item> removed = new List<Item>();
            int starting = Random.Range(0, inventorySlots.Length-4);
            for(int i = 0; i < inventorySlots.Length-4 && numRemove > 0;i++){
                if(inventorySlots[i].myItem != null){
                    InventorySlot slot = inventorySlots[i];
                    removed.Add(slot.myItem.myItem);
                    itemAmts[slot.myItem.myItem] -= slot.myItem.amount;
                    numRemove--;
                    if(slot.myItem.myItem.itemType == ItemType.Potion || slot.myItem.myItem.itemType == ItemType.Armor)
                        numRemove = 0;
                    if(slot.myItem.amount > 1){
                        slot.myItem.amount--;
                        slot.myItem.SetText();
                    }else{
                        slot.myItem.transform.SetParent(null);
                        Destroy(slot.myItem);
                        slot.myItem = null;
                    }
                }
            }
            //IMPLEMENT DIALOGUE BOX WITH ITEMS LOST: temporary below
            string t = "You lost " + removed.Count + " items: ";
            foreach(Item i in removed){
                t += i.name + ", ";
            } 
            Debug.Log(t);
            return;
        }

        if(item && slotNum == -1){ //crafting
            itemAmts[item] -= numRemove;
            for(int i = 0; i < inventorySlots.Length; i++){
                if(inventorySlots[i].myItem != null && inventorySlots[i].myItem.myItem == item){
                    InventorySlot slot = inventorySlots[i];
                    itemAmts[slot.myItem.myItem] -= numRemove;
                    if(slot.myItem.amount > numRemove){
                        slot.myItem.amount-= numRemove;
                        slot.myItem.SetText();
                    }else{
                        slot.myItem.transform.SetParent(null);
                        Destroy(slot.myItem);
                        slot.myItem = null;
                    }
                    return;
                }
            }
        }

        if(slotNum > -1){ //use item
            if(item == null)
                item = inventorySlots[slotNum].myItem.myItem;
            //invalid range
            if(slotNum >= inventorySlots.Length || slotNum < 0){ 
                Debug.Log("slotNum out of range");
                return;
            }
            //slot is empty
            if(inventorySlots[slotNum].myItem == null){
                Debug.Log("slot is empty");
                return;
            }
            //item not usable
            if(item.itemType != ItemType.Potion){
                Debug.Log("item not usable");
                return;
            }
            InventorySlot slot = inventorySlots[slotNum];
            if(numRemove == -1) numRemove = 1;

            //not enough items to remove
            if(!itemAmts.ContainsKey(item) || itemAmts[item] < numRemove){
                Debug.Log("not enough items to remove");
                return;
            }
            
            itemAmts[item] -= numRemove;
            if(slot.myItem.amount > numRemove){
                slot.myItem.amount-= numRemove;
                slot.myItem.SetText();
            }else{
                slot.myItem.transform.SetParent(null);
                Destroy(slot.myItem);
                slot.myItem = null;
            }
        }
    }

    public void view1v1(){
        isActive = !isActive;
        if(isActive){ //now active
            obj.SetActive(isActive);
            for(int i = 0; i < inventorySlots.Length; i++){
                if(inventorySlots[i].myItem != null){
                    //make all unusable items transparent
                    if(inventorySlots[i].myItem.myItem.itemType != ItemType.Potion){
                        Image icon = inventorySlots[i].myItem.itemIcon;
                        Color c = icon.color;
                        c.a = 0.25f; // Set to 25% opacity
                        icon.color = c;
                    }
                    //change background of usable items
                    else{
                        Image slotImage = inventorySlots[i].GetComponentInChildren<Image>();
                        slotImage.sprite = highlightSprite; 
                    }
                }
            }
        }
        else{ //make inactive, everything back to normal
            for(int i = 0; i < inventorySlots.Length; i++){
                if(inventorySlots[i].myItem != null){
                    if(inventorySlots[i].myItem.myItem.itemType != ItemType.Potion){
                        Image icon = inventorySlots[i].myItem.itemIcon;
                        Color c = icon.color;
                        c.a = 1f; // Full opacity
                        icon.color = c;
                    }
                    else{
                        Image slotImage = inventorySlots[i].GetComponentInChildren<Image>();
                        slotImage.sprite = inventorySlots[i].defaultSprite; 
                    }
                }
            }
            obj.SetActive(isActive);
        }
    }
}


