using UnityEngine;
namespace InventorySystem
{
    //Author: Jaxon Schauer
    /// <summary>
    /// This class has test functions, can be used to help debug
    /// </summary>
    public class Test : MonoBehaviour
    {
        [SerializeField]
        private string inventoryName;
        [SerializeField]
        private int position;
        [SerializeField]
        private int testAmount;
        [SerializeField]
        private char testToggleKey;
        [SerializeField]
        private string testItem;
        private bool toggleSwitch;
        // Start is called before the first frame update
        private void Start()
        {
            Debug.Log("Inventory testing enabled, use the following keys to perform tests");
            PrintTests();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TestRemoveItem(position);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TestInventoryDictPos();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TestToggle();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TestAddItem();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                TestAddItemPos();
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                TestCountItems();
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                TestInventoryFull();
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                PrintTests();
            }
        }
        public void PrintTests()
        {
            Debug.Log("1. Test Remove Item");
            Debug.Log("2. Test Inventory Position Dictionary");
            Debug.Log("3. Test Inventory Toggling");
            Debug.Log("4. Test Add Item Linearly");
            Debug.Log("5. Test Add Item At Position");
            Debug.Log("6. Test Count Items");
            Debug.Log("7. Test Inventory Full");
            Debug.Log("0. Print All Tests");

        }
        // Update is called once per frame
        public void TestCountItems()
        {
            int count = InventoryController.instance.CountItems(inventoryName, testItem);
            Debug.Log(testItem + ": " + count);
        }
        public void RemoveItem(InventoryItem itemToRemove)
        {
            InventoryController.instance.RemoveItem(itemToRemove.GetInventory(), itemToRemove, testAmount);
        }
        public void TestRemoveItem(int pos)
        {
            InventoryController.instance.RemoveItem(inventoryName, testItem, testAmount);
        }
        public void TestInventoryDictPos()
        {
            Inventory inventory = InventoryController.instance.GetInventory(inventoryName);
            inventory.TestPrintItemPosDict();
        }
        public void TestToggle()
        {
            if (toggleSwitch)
            {
                InventoryController.instance.AddToggleKey(inventoryName, testToggleKey);
                toggleSwitch = false;
            }
            else
            {
                InventoryController.instance.RemoveToggleKey(inventoryName, testToggleKey);
                toggleSwitch = true;
            }
        }
        public void TestAddItem()
        {
            InventoryController.instance.AddItem(inventoryName, testItem, testAmount);

        }
        public void TestAddItemPos()
        {
            InventoryController.instance.AddItemPos(inventoryName, testItem, position, testAmount);

        }
        public void TestInventoryFull()
        {
            Debug.Log(InventoryController.instance.GetInventory(inventoryName).Full(testItem));
        }
        public void PrintItemData(InventoryItem item)
        {
            Debug.Log(item);
        }
    }
}
