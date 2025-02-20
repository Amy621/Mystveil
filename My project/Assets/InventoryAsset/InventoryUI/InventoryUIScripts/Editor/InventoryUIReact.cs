using UnityEditor;
using UnityEngine;
namespace InventorySystem
{
    
    /// <summary>
    /// This updates the inventory ui when the inspector for the inventorUImanager changes.
    /// </summary>
    
    [CustomEditor(typeof(InventoryUIManager))]
    internal class InventoryUIReact : Editor
    {
        private bool needToUpdate = false;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            // Draw default inspector content.
            DrawDefaultInspector();

            // Check if anything was changed.
            if (EditorGUI.EndChangeCheck())
            {
                needToUpdate = true;
            }

            // If any changes in the inspector.
            if (needToUpdate)
            {
                // Reference to the InventoryUI script.
                InventoryUIManager inventoryUI = (InventoryUIManager)target;

                // Apply changes to inventory display.
                inventoryUI.UpdateInventoryUI();

                // Mark object as dirty.
                EditorUtility.SetDirty(inventoryUI);

                needToUpdate = false;
            }
        }
    }
}