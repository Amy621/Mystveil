using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    
    /// <summary>
    /// Holds information given from the drop down menu 
    /// </summary>
    
    public class AddItem : MonoBehaviour
    {
        [SerializeField]
        private int amount;
        [HideInInspector]
        private List<ItemInitializer> items;

        [HideInInspector]
        private List<InventoryInitializer> inventories;

        [HideInInspector]
        public int selectedItemIndex = 0;

        [HideInInspector]
        public int SelectedInventoryIndex = 0;
        [SerializeField, HideInInspector]
        private ItemInitializer item;
        [SerializeField, HideInInspector]
        private InventoryInitializer inventory;
        [SerializeField, HideInInspector]
        GameObject controller;

        public void FindController()
        {
            controller = GameObject.Find("InventoryController");

        }
        public void FindItemList()
        {
            if (controller == null)
            {
                FindController();
            }
            items = controller.GetComponent<InventoryController>().items;
        }
        public void FindInventoryList()
        {
            if (controller == null)
            {
                FindController();
            }
            inventories = controller.GetComponent<InventoryController>().initializeInventory;
        }
        private void Awake()
        {
            gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            if (InventoryController.instance == null)
            {
                Destroy(gameObject);
            }
            if (amount != 0)
            {
                InventoryController.instance.AddItem(inventory.GetInventoryName(), item.GetItemType(), amount);
            }
            else
            {
                InventoryController.instance.AddItem(inventory.GetInventoryName(), item.GetItemType());
            }
            gameObject.SetActive(false);
        }
        public void SetItem(ItemInitializer init)
        {
            item = init;
        }
        public void SetInventory(InventoryInitializer init)
        {
            inventory = init;
        }
        public void SetInventoryList(List<InventoryInitializer> init)
        {
            inventories = init;
        }
        public List<InventoryInitializer> GetInvList()
        {
            return inventories;
        }
        public void SetItemList(List<ItemInitializer> init)
        {
            items = init;
        }
        public List<ItemInitializer> GetItemsList()
        {
            return items;
        }
    }

}

