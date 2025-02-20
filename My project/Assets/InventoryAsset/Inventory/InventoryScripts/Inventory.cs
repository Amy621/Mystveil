   
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace InventorySystem
{
    
    /// <summary>
    /// This class creates an Inventory that tracks and controls the inventory list. This class tells the InventoryUIManager what objects each slot holds
    /// </summary>
    
    [System.Serializable]
    public class Inventory
    {
        private Dictionary<string, List<int>> itemPositions;//Holds all positions of a given itemType in the list

        private List<InventoryItem> inventoryList;//Holds all inventory items in a list

        [SerializeField, HideInInspector]
        private string inventoryName;
        [SerializeField, HideInInspector]
        private GameObject InventoryUIManager;//holds the linked InventoryUIManager GameObject
        [SerializeField, HideInInspector]
        InventoryUIManager InventoryUIManagerInstance;//Holds an instance of the linked InventoryUIManager class
        [SerializeField, HideInInspector]
        int size;
        [SerializeField, HideInInspector]
        bool saveInventory;//is true if the user decides to save the inventory
        private bool acceptAll;
        private bool rejectAll;
        private HashSet<string> exceptions;
        private Dictionary<int, InventoryItemEvent> enterDict;
        private Dictionary<int, InventoryItemEvent> exitDict;
        private Dictionary<int, bool> itemAction;

        /// <summary>
        /// Assigns essential variables for the Inventory
        /// </summary>
        public Inventory(GameObject InventoryUIManager, string name, int size)
        {
            this.InventoryUIManager = InventoryUIManager;
            this.inventoryName = name;
            inventoryList = new List<InventoryItem>(size);
            this.size = size;
            FillInventory(size);
            InventoryUIManagerInstance = InventoryUIManager.GetComponent<InventoryUIManager>();
        }

        /// <summary>
        /// Initializes aspects of the inventory that do not transfer into play mode.
        /// </summary>
        public void Init()
        {
            exceptions = new HashSet<string>();
            itemPositions = new Dictionary<string, List<int>>();
        }
        public void InitList()
        {
            inventoryList = new List<InventoryItem>(size);
            FillInventory(size);
            Resize(size);
        }

        /// <summary>
        /// Resizes the inventory when <see cref="InventoryUIManager.UpdateInventoryUI"/> is called
        /// </summary>
        public void Resize(int newSize)
        {
            if (inventoryList != null)
            {
                itemPositions = new Dictionary<string, List<int>>();
                List<InventoryItem> newlist = new List<InventoryItem>();

                if (size < newSize)
                {
                    for (int i = 0; i < inventoryList.Count; i++)
                    {
                        InventoryItem item = inventoryList[i];
                        newlist.Add(item);
                        AddItemHelper(item, i, false);

                    }
                    for (int i = newlist.Count; i < newSize; i++)
                    {
                        InventoryItem filler = new InventoryItem(true);
                        newlist.Add(filler);
                        AddItemHelper(filler, i, false);

                    }
                }
                else
                {
                    for (int i = 0; i < newSize; i++)
                    {
                        InventoryItem item = inventoryList[i];
                        newlist.Add(item);
                        AddItemHelper(item, i, false);

                    }
                }
                inventoryList.Clear();

                inventoryList = newlist;

            }
            size = newSize;
        }
        /// <summary>
        /// Empties inventory
        /// </summary>
        public void Clear()
        {
            int count = 0;
            Dictionary<string, List<int>> copy = new Dictionary<string, List<int>>();

            foreach (var kvp in itemPositions)
            {
                copy[kvp.Key] = new List<int>(kvp.Value);
            }
            foreach (KeyValuePair<string,List<int>> pair in copy)
            {
                count++;
                if(pair.Key == "Empty")
                {
                    continue;
                }
                List<int> items = pair.Value;
                foreach(int pos in items)
                {
                    EraseItemInPosition(pos);
                }
                if(count == size)
                {
                    return;
                }
            }
            
        }
        /// <summary>
        /// Adds an item to a specified position, updating the <see cref="itemPositions"/> for efficient tracking of the inventory items
        /// </summary>
        public void AddItemPos(int index, InventoryItem item)
        {
            if (inventoryList == null)
            {
                Debug.LogError("Items List Null");
                return;
            }
            else if (index > size - 1)
            {
                Debug.LogWarning("Out of Range Adding to closest Index: " + index);
                index = size - 1;
            }
            else if (index < 0)
            {
                Debug.LogWarning("Out of Range Adding to Closest Index: " + index);
                index = 0;
            }
            if (!CheckAcceptance(item.GetItemType()))
            {
                Debug.LogWarning("Item Acceptance is false. Overruling and adding item.");
            }
            InventoryItem newItem = new InventoryItem(item, item.GetAmount());
            InventoryItem curItem = inventoryList[index];

            if (curItem.GetIsNull())
            {
                AddItemHelper(newItem, index);
            }
            else
            {

                if (curItem.GetItemType() == newItem.GetItemType())
                {
                    if (curItem.GetAmount() + newItem.GetAmount() <= curItem.GetItemStackAmount())
                    {
                        curItem.SetAmount(curItem.GetAmount() + newItem.GetAmount());
                        InventoryUIManagerInstance.UpdateSlot(index);
                    }
                    else
                    {
                        AddItemAuto(newItem, newItem.GetAmount());
                    }
                }
                else
                {
                    AddItemAuto(newItem, newItem.GetAmount());
                }
            }
        }

        /// <summary>
        /// Takes an item as input
        /// Adds the item at the lowest possible inventory location, adding it into the <see cref="itemPositions"/> to allow for efficient tracking of the inventory items
        /// </summary>
        public void AddItemAuto(InventoryItem item, int amount = 1)
        {
            if (!CheckAcceptance(item.GetItemType()))
            {
                Debug.LogWarning("Inventory is set to not accept this item. Overruling and adding item.");
            }
            InventoryItem newItem = new InventoryItem(item, amount);
            if (itemPositions.ContainsKey(newItem.GetItemType()))
            {
                for (int i = 0; i < itemPositions[newItem.GetItemType()].Count; i++)
                {
                    int position = itemPositions[newItem.GetItemType()][i];
                    InventoryItem curItem = inventoryList[position];
                    if (curItem.GetItemStackAmount() >= curItem.GetAmount())
                    {
                        int diff = curItem.GetItemStackAmount() - curItem.GetAmount();
                        if(newItem.GetAmount() > diff)
                        {
                            curItem.SetAmount(inventoryList[position].GetAmount() + diff);
                            newItem.SetAmount(newItem.GetAmount() - diff);
                        }
                        else
                        {
                            curItem.SetAmount(inventoryList[position].GetAmount() + newItem.GetAmount());
                            InventoryUIManagerInstance.UpdateSlot(position);
                            return;

                        }
                        InventoryUIManager.GetComponent<InventoryUIManager>().UpdateSlot(position);
                    }
                }
                AddLinearly(newItem, newItem.GetAmount());
            }
            else
            {
                AddLinearly(newItem, newItem.GetAmount());
            }

        }

        /// <summary>
        /// Adds a new item in the lowest possible inventoryList position
        /// </summary>
        private void AddLinearly(InventoryItem item, int amount = 1)
        {
            int trackAmount = amount;
            for (int i = 0; i < inventoryList.Count; i++)
            {

                if (inventoryList[i].GetIsNull())
                {
                    if(item.GetItemStackAmount() < trackAmount)
                    {
                        InventoryItem newItem = new InventoryItem(item, item.GetItemStackAmount());
                        AddItemHelper(newItem, i);
                        trackAmount = trackAmount - item.GetItemStackAmount();

                    }
                    else
                    {
                        InventoryItem newItem = new InventoryItem(item, trackAmount);
                        AddItemHelper(newItem, i);
                        trackAmount = trackAmount - amount;

                    }
                    if (trackAmount <= 0)
                    {
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Adds itemstypes into <see cref="itemPositions"/> and tracks their positions for quick add/remove and count functions.
        /// </summary>
        private void AddItemHelper(InventoryItem item, int pos, bool invokeEnterExit = true)
        {
            string empty = "Empty";
            if (!item.GetIsNull())
            {

                InventoryItem newItem = new InventoryItem(item, item.GetAmount());

                newItem.SetPosition(pos);
                newItem.SetInventory(inventoryName);
                inventoryList[pos] = newItem;

                if (!itemPositions.ContainsKey(item.GetItemType()))
                {

                    if (itemPositions.ContainsKey(empty))
                    {
                        itemPositions[empty].Remove(pos);
                    }
                    itemPositions.Add(item.GetItemType(), new List<int>());
                    itemPositions[item.GetItemType()].Add(pos);

                    InventoryUIManagerInstance.UpdateSlot(pos);
                }
                else
                {
                    if (itemPositions.ContainsKey(empty))
                    {
                        itemPositions[empty].Remove(pos);
                    }
                    itemPositions[item.GetItemType()].Add(pos);
                    InventoryUIManagerInstance.UpdateSlot(pos);
                }
                if (invokeEnterExit && enterDict != null && enterDict.ContainsKey(pos))
                {
                    if (itemAction[pos])
                    {
                        newItem.Selected();
                    }
                    enterDict[pos].Invoke(newItem);
                }

            }
            else
            {
                if (!itemPositions.ContainsKey(empty))
                {
                    itemPositions.Add(empty, new List<int>() { pos });
                }
                else
                {
                    itemPositions[empty].Add(pos);
                }
            }

        }

        /// <summary>
        /// Takes as input a position, remove the item from the given inventory position.
        /// </summary>
        public void EraseItemInPosition(int pos, bool invokeEnterExit = true)
        {
            RemoveItemHelper(inventoryList[pos], pos, invokeEnterExit);
        }

        /// <summary>
        /// Removes items in a specified position.
        /// </summary>
        public void RemoveItemInPosition(int pos, int amount)
        {
            InventoryItem item = inventoryList[pos];
            if (!item.GetIsNull())
            {
                if (itemPositions.ContainsKey(item.GetItemType()))
                {
                    if (item.GetAmount() - amount > 0)
                    {
                        item.SetAmount(item.GetAmount() - amount);
                        InventoryUIManagerInstance.UpdateSlot(pos);

                    }
                    else
                    {
                        RemoveItemHelper(item, pos);
                    }
                }
                else
                {
                    Debug.LogWarning("ItemPositions Dictitonary Setup Incorrectly");
                }
            }
        }

        /// <summary>
        /// Removes items in a specified position, given the item as input
        /// </summary>
        public void RemoveItemInPosition(InventoryItem item, int amount)
        {
            int pos = item.GetPosition();
            if (!item.GetIsNull())
            {
                if (itemPositions.ContainsKey(item.GetItemType()))
                {
                    if (item.GetAmount() - amount > 0)
                    {
                        item.SetAmount(item.GetAmount() - amount);
                        InventoryUIManagerInstance.UpdateSlot(pos);
                    }
                    else
                    {
                        RemoveItemHelper(item, pos);
                    }
                }
                else
                {
                    Debug.LogWarning("ItemPositions Dictitonary Setup Incorrectly");
                }
            }
        }
        public void RemoveItemAuto(string itemType, int amount)
        {
            if(!itemPositions.ContainsKey(itemType))
            {
                Debug.Log("No items of type " + itemType + " in " + inventoryName);
                return;
            }
            List<int> positions = itemPositions[itemType];
            List<int> delpos = new List<int>();
            int trackAmount = amount;
            foreach (int position in positions)
            {
                InventoryItem curItem = inventoryList[position];
                if (trackAmount-curItem.GetAmount() < 0)
                {
                    curItem.SetAmount(inventoryList[position].GetAmount()-trackAmount);
                    InventoryUIManagerInstance.UpdateSlot(position);
                    break;
                }
                trackAmount -= curItem.GetAmount();
                delpos.Add(position);
            }
            foreach(int position in delpos)
            {
                RemoveItemHelper(itemType, position);
            }
            
        }
        /// <summary>
        /// Handles item when it needs to be erased from inventory
        /// </summary>
        public void RemoveItemHelper(InventoryItem item, int pos, bool invokeEnterExit = true)
        {
            string empty = "Empty";
            itemPositions[item.GetItemType()].Remove(pos);
            if (itemPositions.ContainsKey(empty))
            {
                itemPositions[empty].Add(pos);

            }
            InventoryItem filler = new InventoryItem(true);

            inventoryList[pos] = filler;
            InventoryUIManagerInstance.UpdateSlot(pos);
            if (invokeEnterExit && exitDict != null && exitDict.ContainsKey(pos))
            {
                exitDict[pos].Invoke(inventoryList[pos]);
            }
            InventoryUIManagerInstance.UpdateSlot(pos);

        }
        /// <summary>
        /// Handles item when it needs to be erased from inventory
        /// </summary>
        public void RemoveItemHelper(string item, int pos, bool invokeEnterExit = true)
        {
            string empty = "Empty";
            itemPositions[item].Remove(pos);
            if (itemPositions.ContainsKey(empty))
            {
                itemPositions[empty].Add(pos);

            }
            InventoryItem filler = new InventoryItem(true);

            inventoryList[pos] = filler;
            InventoryUIManagerInstance.UpdateSlot(pos);
            if (invokeEnterExit && exitDict != null && exitDict.ContainsKey(pos))
            {
                exitDict[pos].Invoke(inventoryList[pos]);
            }
            InventoryUIManagerInstance.UpdateSlot(pos);

        }

        /// <summary>
        /// returns the count of a input item
        /// </summary>
        public int Count(string itemType)
        {
            if (itemType == null)
            {
                Debug.LogError("String null. Returning 0");
                return 0;
            }
            if (!itemPositions.ContainsKey(itemType))
            {
                Debug.LogError("ItemPositions does not contain itemType: " + itemType + ". Returning 0");
                return 0;
            }
            List<int> items = itemPositions[itemType];
            int itemsTotal = 0;
            foreach (int item in items)
            {
                itemsTotal += inventoryList[item].GetAmount();
            }
            return itemsTotal;
        }
        /// <summary>
        /// takes a string(itemType) as input. Checks whether or not the inventory has room for a item
        /// </summary>
        public bool Full(string item)
        {
            string empty = "Empty";
            if (item == null)
            {
                return true;
            }
            if (itemPositions[empty].Count > 0)
            {
                return false;
            }
            //If inventory has no empty slots and does not yet contain the item
            if (!itemPositions.ContainsKey(item))
            {
                return true;
            }
            List<int> itemPos = itemPositions[item];

            foreach (int pos in itemPos)
            {
                InventoryItem curItem = inventoryList[pos];
                if (curItem.GetAmount() < curItem.GetItemStackAmount())
                {
                    return false;
                }

            }
            return true;
        }

        /// <summary>
        /// Fills the inventory with empty items 
        /// </summary>
        public void FillInventory(int size, bool highlightable = false)
        {
            if (inventoryList == null)
            {
                return;
            }
            for (int i = 0; i < size; i++)
            {
                InventoryItem filler = new InventoryItem(true);
                filler.SetPressable(highlightable);
                inventoryList.Add(filler);
            }
        }

        /// <summary>
        /// Returns the item at a specific index of the inventory, returning the closest value if out of range
        /// </summary>
        public InventoryItem InventoryGetItem(int index)
        {
            if (inventoryList == null)
            {
                Debug.LogError("Items List Null");
                return null;
            }
            else if (index > size - 1)
            {
                Debug.LogWarning("Out of Range Returning Closest Item: " + index);
                return inventoryList[size - 1];
            }
            else if (index < 0)
            {
                Debug.LogWarning("Out of Range Returning Closest Item: " + index);
                return inventoryList[0];
            }
            return inventoryList[index];
        }

        /// <summary>
        /// Sets up values for the inventory to determine if an item should be accepted or rejected from the inventory
        /// </summary>
        public void SetupItemAcceptance(bool acceptAll, bool rejectAll, List<string> exceptions)
        {
            if (acceptAll && !rejectAll)
            {
                this.acceptAll = true;
                this.rejectAll = false;
            }
            else if (rejectAll && !acceptAll)
            {
                this.acceptAll = false;
                this.rejectAll = true;
            }
            else
            {
                Debug.LogError("Only one AcceptAll or RejectAll should Be True And False");
            }
            foreach (string exception in exceptions)
            {
                if (!this.exceptions.Contains(exception))
                {
                    this.exceptions.Add(exception);
                }
                else
                {
                    Debug.LogWarning("No Duplicate Items Should Exist In Exception List");
                }
            }
        }
        /// <summary>
        /// Prints <see cref="itemPositions"/>
        /// </summary>
        public Dictionary<string, List<int>> TestPrintItemPosDict()
        {
            StringBuilder output = new StringBuilder();

            output.Append(itemPositions.Count + " | ");

            foreach (KeyValuePair<string, List<int>> pair in itemPositions)
            {
                output.Append(pair.Key + ": ");
                foreach (int position in pair.Value)
                {
                    output.Append(position + " ");
                }
                output.Append("| ");
            }

            Debug.Log(output.ToString());
            return itemPositions;
        }

        /// <summary>
        /// Returns a bool, true if an item can be transfered into an inventory and false otherwise.
        /// </summary>
        public bool CheckAcceptance(string itemType)
        {
            if ((acceptAll && rejectAll) || (!acceptAll && !rejectAll))
            {
                Debug.LogWarning("Acceptance Incorrectly Setup, Returning True Or False For All, and should only be for one. Return True for All.");
                return true;
            }
            if (acceptAll && !exceptions.Contains(itemType))
            {
                return true;
            }
            else if (rejectAll && exceptions.Contains(itemType))
            {
                return true;
            }
            return false;
        }
        public void SetSave(bool saveable)
        {
            this.saveInventory = saveable;
        }
        public string GetName()
        {
            return inventoryName;
        }
        public void SetManager(GameObject manager)
        {
            this.InventoryUIManager = manager;
        }
        public List<InventoryItem> GetList()
        {
            return inventoryList;
        }
        public bool GetSaveInventory()
        {
            return saveInventory;
        }
        public void SetExitEntranceDict(Dictionary<int, InventoryItemEvent> enterDict, Dictionary<int, InventoryItemEvent> exitDict, Dictionary<int, bool> itemAction)
        {
            this.enterDict = enterDict;
            this.exitDict = exitDict;
            this.itemAction = itemAction;
        }

    }
}