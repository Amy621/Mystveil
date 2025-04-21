using System.Collections;
using System.Collections.Generic;
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
        if(!Inventory.Singleton.isActive){
            if(Input.GetKeyDown(KeyCode.Alpha1)){
            removeItems(null, 1, inventorySlots[42]);
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)){
            removeItems(null, 1, inventorySlots[43]);
            }
            if(Input.GetKeyDown(KeyCode.Alpha3)){
            removeItems(null, 1, inventorySlots[44]);
            }
            if(Input.GetKeyDown(KeyCode.Alpha4)){
            removeItems(null, 1, inventorySlots[45]);
            }
            if(Input.GetKeyDown(KeyCode.Alpha5)){
            removeItems(null, 1, inventorySlots[46]);
            }
            if(Input.GetKeyDown(KeyCode.Alpha6)){
            removeItems(null, 1, inventorySlots[47]);
            }
            if(Input.GetKeyDown(KeyCode.Alpha7)){
            removeItems(null, 1, inventorySlots[48]);
            }

            if(Input.GetKeyDown(KeyCode.Alpha8)){
            removeItems(null, 1, inventorySlots[49]);
            }

            if(Input.GetKeyDown(KeyCode.Alpha9)){
            removeItems(null, 1, inventorySlots[50]);
            }

            if(Input.GetKeyDown(KeyCode.Alpha0)){
            removeItems(null, 1, inventorySlots[51]);
            }
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
        if(slot == null || slot.myItem == null) return; //cant combine empty or non-stackable items
        Item item = slot.myItem.myItem;

        int firstEmpty = -1;
        int firstEmptyHb = -1;
        //equip armor
        if(item.itemType == ItemType.Armor && slot.myTag == SlotTag.None){
            int armorSlot = -1;
            if(item.slotTag == SlotTag.Head && inventorySlots[52].myItem == null)
                armorSlot = 52;
            else if(item.slotTag == SlotTag.Body && inventorySlots[53].myItem == null)
                armorSlot = 53;
            else if(item.slotTag == SlotTag.Wand && inventorySlots[54].myItem == null)
                armorSlot = 54;
            else if(item.slotTag == SlotTag.Accessory && inventorySlots[55].myItem == null)
                armorSlot = 55;

            if(armorSlot != -1){
                inventorySlots[armorSlot].SetItem(slot.myItem);
                return;
            }
        }
        int slotNum = -1;
        for(int i = 0; i < 52; i++){
            if(slot == inventorySlots[i]){
                slotNum = i;
                continue;
            }
            if(inventorySlots[i].myItem != null && inventorySlots[i].myItem.myItem == item && inventorySlots[i] != slot){
                //cant stack multiple in armor slots
                if(inventorySlots[i].myTag != SlotTag.None && slot.myTag == SlotTag.None)
                    continue;
                //stack in found slot
                inventorySlots[i].myItem.amount += slot.myItem.amount;
                inventorySlots[i].myItem.SetText();
                slot.myItem.transform.SetParent(null);
                Destroy(slot.myItem); 
                slot.myItem = null;
                if(inventorySlots[i].myTag != SlotTag.None) 
                    inventorySlots[i].changeImage();
                if(slot.myTag != SlotTag.None) 
                    slot.changeImage();
                return;
            }else if(i >= 42 && inventorySlots[i].myItem == null && firstEmptyHb == -1)
                firstEmptyHb = i; //find first empty hotbar slot
            else if(firstEmpty == -1 &&inventorySlots[i].myItem == null){
                firstEmpty = i;
            }
        }
        if(firstEmpty == -1 && firstEmptyHb == -1) return;
        if(firstEmptyHb != -1 && slotNum < 42)
            firstEmpty = firstEmptyHb;
        InventorySlot newSlot = inventorySlots[firstEmpty];
        newSlot.SetItem(slot.myItem); //create item in slot
        if(slot.myTag != SlotTag.None) 
            slot.changeImage();
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
    public void removeItems(Item item = null, int numRemove = -1, InventorySlot slot = null){
        if(item && (!itemAmts.ContainsKey(item) || itemAmts[item] < numRemove)){
            Debug.Log("not enough items to remove");
            return;
        }
        if(item == null && slot == null){ //find item(s) to remove
            List<Item> removed = new List<Item>();
            int starting = Random.Range(0, inventorySlots.Length-4);
            for(int i = 0; i < inventorySlots.Length-4 && numRemove > 0;i++){
                if(inventorySlots[i].myItem != null){
                    slot = inventorySlots[i];
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

        if(item && slot == null){ //crafting
            itemAmts[item] -= numRemove;
            for(int i = 0; i < inventorySlots.Length; i++){
                if(inventorySlots[i].myItem != null && inventorySlots[i].myItem.myItem == item){
                    slot = inventorySlots[i];
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

        if(slot != null){ //use item
            //slot empty
            if(slot.myItem == null){
                Debug.Log("slot is empty");
                return;
            }
            if(item == null)
                item = slot.myItem.myItem;
            //item not usable
            if(item.itemType != ItemType.Potion){
                Debug.Log("item not usable");
                return;
            }
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

    public bool hasPotions(){
        for(int i = 0; i < inventorySlots.Length; i++){
            if(inventorySlots[i].myItem != null && inventorySlots[i].myItem.myItem.itemType == ItemType.Potion){
                return true;
            }
        }
        return false;
    }
}
