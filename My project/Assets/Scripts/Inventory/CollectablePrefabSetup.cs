using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class CollectablePrefabSetup : EditorWindow
{
    private GameObject prefab;
    private Item itemData;
    private int quantity = 1;
    private bool destroyOnPickup = true;
    private AudioClip pickupSound;
    private GameObject pickupEffect;
    
    [MenuItem("Tools/Inventory/Collectable Prefab Setup")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CollectablePrefabSetup), false, "Collectable Setup");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Collectable Item Prefab Setup", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab:", prefab, typeof(GameObject), false);
        itemData = (Item)EditorGUILayout.ObjectField("Item Data:", itemData, typeof(Item), false);
        quantity = EditorGUILayout.IntField("Quantity:", quantity);
        destroyOnPickup = EditorGUILayout.Toggle("Destroy On Pickup:", destroyOnPickup);
        pickupSound = (AudioClip)EditorGUILayout.ObjectField("Pickup Sound:", pickupSound, typeof(AudioClip), false);
        pickupEffect = (GameObject)EditorGUILayout.ObjectField("Pickup Effect:", pickupEffect, typeof(GameObject), false);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Apply to Prefab"))
        {
            if (prefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a prefab first!", "OK");
                return;
            }
            
            if (itemData == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select an item data asset!", "OK");
                return;
            }
            
            SetupPrefab();
        }
    }
    
    private void SetupPrefab()
    {
        // Open the prefab for editing
        string prefabPath = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
        
        try
        {
            // Check if the prefab already has a CollectableItem component
            CollectableItem collectableItem = prefabInstance.GetComponent<CollectableItem>();
            if (collectableItem == null)
            {
                collectableItem = prefabInstance.AddComponent<CollectableItem>();
            }
            
            // Set the fields via reflection since they're private
            var itemDataField = collectableItem.GetType().GetField("itemData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var quantityField = collectableItem.GetType().GetField("quantity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destroyField = collectableItem.GetType().GetField("destroyOnPickup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var soundField = collectableItem.GetType().GetField("pickupSound", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var effectField = collectableItem.GetType().GetField("pickupEffect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (itemDataField != null) itemDataField.SetValue(collectableItem, itemData);
            if (quantityField != null) quantityField.SetValue(collectableItem, quantity);
            if (destroyField != null) destroyField.SetValue(collectableItem, destroyOnPickup);
            if (soundField != null) soundField.SetValue(collectableItem, pickupSound);
            if (effectField != null) effectField.SetValue(collectableItem, pickupEffect);
            
            // Make sure the prefab has a collider
            Collider existingCollider = prefabInstance.GetComponent<Collider>();
            if (existingCollider == null)
            {
                SphereCollider sphereCollider = prefabInstance.AddComponent<SphereCollider>();
                sphereCollider.isTrigger = true;
                sphereCollider.radius = 1.5f; // Generous pickup radius
            }
            else if (!existingCollider.isTrigger)
            {
                existingCollider.isTrigger = true;
            }
            
            // Set the tag to "Collectable"
            prefabInstance.tag = "Collectable";
            
            // Save the changes back to the prefab
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            
            EditorUtility.DisplayDialog("Success", "Applied collectable setup to prefab!", "OK");
        }
        finally
        {
            // Always unload the prefab contents
            PrefabUtility.UnloadPrefabContents(prefabInstance);
        }
    }
}
#endif 