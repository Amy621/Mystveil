using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class AttachCollectableComponents : MonoBehaviour
{
    [MenuItem("Tools/Inventory/Attach Collectable Components")]
    public static void AttachComponents()
    {
        // Find all objects with the "Collectable" tag
        GameObject[] collectables = GameObject.FindGameObjectsWithTag("Collectable");
        int count = 0;
        
        foreach (GameObject collectable in collectables)
        {
            // Skip if it already has the component
            if (collectable.GetComponent<CollectableItem>() != null)
                continue;
                
            // Get the base name without "(Clone)" suffix
            string baseName = collectable.name.Replace("(Clone)", "").Trim();
            
            // Find the matching Item asset
            string[] guids = AssetDatabase.FindAssets("t:Item " + baseName);
            Item itemAsset = null;
            
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                itemAsset = AssetDatabase.LoadAssetAtPath<Item>(path);
            }
            
            // Add the CollectableItem component
            CollectableItem collectableItem = collectable.AddComponent<CollectableItem>();
            
            // Set the fields via reflection since they're private
            var itemDataField = collectableItem.GetType().GetField("itemData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (itemDataField != null && itemAsset != null)
            {
                itemDataField.SetValue(collectableItem, itemAsset);
            }
            
            // Make sure it has a collider set to trigger
            Collider existingCollider = collectable.GetComponent<Collider>();
            if (existingCollider == null)
            {
                SphereCollider sphereCollider = collectable.AddComponent<SphereCollider>();
                sphereCollider.isTrigger = true;
                sphereCollider.radius = 1.5f; // Generous pickup radius
            }
            else if (!existingCollider.isTrigger)
            {
                existingCollider.isTrigger = true;
            }
            
            count++;
        }
        
        Debug.Log($"Added CollectableItem component to {count} collectible objects");
    }
}
#endif 