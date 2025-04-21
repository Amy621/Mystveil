using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(InventoryBridge))]
public class InventoryBridgeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Sync Inventory Now"))
        {
            InventoryBridge bridge = (InventoryBridge)target;
            bridge.SyncToUIInventory();
        }
    }
    
    [MenuItem("Tools/Inventory/Add Manager to UI Inventory")]
    public static void AddInventoryManagerToUI()
    {
        InventoryBridge.AddInventoryManagerToUI();
    }
}
#endif 