using UnityEditor;
namespace InventorySystem
{
    /// <summary>
    /// Creates the dropdown menu on the AddItem.
    /// </summary>
    
    [CustomEditor(typeof(AddItem))]
    public class AddItemReact : Editor
    {
        public override void OnInspectorGUI()
        {

            // Draw the default inspector
            DrawDefaultInspector();
            InventoryDropDown();
            ItemDropDown();


        }
        private void InventoryDropDown()
        {
            AddItem script = (AddItem)target;

            script.FindInventoryList();

            if (script.GetInvList() != null && script.GetInvList().Count > 0)
            {
                string[] inventoryNames = new string[script.GetInvList().Count];
                for (int i = 0; i < script.GetInvList().Count; i++)
                {
                    inventoryNames[i] = script.GetInvList()[i].GetInventoryName();
                }
                script.SelectedInventoryIndex = EditorGUILayout.Popup("Select Inventory", script.SelectedInventoryIndex, inventoryNames);
                script.SetInventory(script.GetInvList()[script.SelectedInventoryIndex]);
                EditorUtility.SetDirty(script);
            }
            else
            {
                EditorGUILayout.LabelField("No Inventories found.");
            }
        }
        private void ItemDropDown()
        {
            AddItem script = (AddItem)target;
            script.FindItemList();

            // If there are items in the list, show the dropdown
            if (script.GetItemsList() != null && script.GetItemsList().Count > 0)
            {
                string[] itemNames = new string[script.GetItemsList().Count];
                for (int i = 0; i < script.GetItemsList().Count; i++)
                {
                    itemNames[i] = script.GetItemsList()[i].GetItemType();
                }

                script.selectedItemIndex = EditorGUILayout.Popup("Select Item", script.selectedItemIndex, itemNames);

                script.SetItem(script.GetItemsList()[script.selectedItemIndex]);
                EditorUtility.SetDirty(script);


            }
            else
            {
                EditorGUILayout.LabelField("No items found.");
            }
        }
    }
}
