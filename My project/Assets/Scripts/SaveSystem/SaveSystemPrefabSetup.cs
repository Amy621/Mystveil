using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
/// <summary>
/// Editor utility to create a save system prefab with all required components.
/// </summary>
public class SaveSystemPrefabSetup : MonoBehaviour
{
    [MenuItem("Tools/Save System/Create Save System Prefab")]
    public static void CreateSaveSystemPrefab()
    {
        // Create the save system root GameObject
        GameObject saveSystemObj = new GameObject("SaveSystem");
        
        // Add required components
        saveSystemObj.AddComponent<SaveManager>();
        saveSystemObj.AddComponent<GameManager>();
        saveSystemObj.AddComponent<QuestManager>();
        saveSystemObj.AddComponent<EnemyDropManager>();
        
        // Create the prefab
        string prefabPath = "Assets/Prefabs/SaveSystem.prefab";
        
        // Ensure directory exists
        if (!System.IO.Directory.Exists("Assets/Prefabs"))
        {
            System.IO.Directory.CreateDirectory("Assets/Prefabs");
        }
        
        // Create prefab
        #if UNITY_2018_3_OR_NEWER
        // For Unity 2018.3 and newer
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(saveSystemObj, prefabPath);
        #else
        // For older Unity versions
        GameObject prefab = PrefabUtility.CreatePrefab(prefabPath, saveSystemObj);
        #endif
        
        Debug.Log("Save System prefab created at: " + prefabPath);
        
        // Cleanup
        DestroyImmediate(saveSystemObj);
        
        // Select the created prefab
        Selection.activeObject = prefab;
    }
    
    [MenuItem("Tools/Save System/Create Required Scriptable Objects")]
    public static void CreateRequiredScriptableObjects()
    {
        // Create directories if they don't exist
        if (!System.IO.Directory.Exists("Assets/ScriptableObjects"))
        {
            System.IO.Directory.CreateDirectory("Assets/ScriptableObjects");
        }
        
        if (!System.IO.Directory.Exists("Assets/ScriptableObjects/Databases"))
        {
            System.IO.Directory.CreateDirectory("Assets/ScriptableObjects/Databases");
        }
        
        // Create Quest Database if it doesn't exist
        string questDBPath = "Assets/ScriptableObjects/Databases/QuestDatabase.asset";
        if (!System.IO.File.Exists(questDBPath))
        {
            QuestDatabase questDB = ScriptableObject.CreateInstance<QuestDatabase>();
            AssetDatabase.CreateAsset(questDB, questDBPath);
            Debug.Log("Quest Database created at: " + questDBPath);
        }
        
        // Create Item Database if it doesn't exist
        string itemDBPath = "Assets/ScriptableObjects/Databases/ItemDatabase.asset";
        if (!System.IO.File.Exists(itemDBPath))
        {
            ItemDatabase itemDB = ScriptableObject.CreateInstance<ItemDatabase>();
            AssetDatabase.CreateAsset(itemDB, itemDBPath);
            Debug.Log("Item Database created at: " + itemDBPath);
        }
        
        // Create Spell Database if it doesn't exist
        string spellDBPath = "Assets/ScriptableObjects/Databases/SpellDatabase.asset";
        if (!System.IO.File.Exists(spellDBPath))
        {
            SpellDatabase spellDB = ScriptableObject.CreateInstance<SpellDatabase>();
            AssetDatabase.CreateAsset(spellDB, spellDBPath);
            Debug.Log("Spell Database created at: " + spellDBPath);
        }
        
        // Refresh AssetDatabase
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Tools/Save System/Setup Scene")]
    public static void SetupScene()
    {
        // Create SaveSystemSetup GameObject
        GameObject saveSystemSetupObj = new GameObject("SaveSystemSetup");
        SaveSystemSetup setupComponent = saveSystemSetupObj.AddComponent<SaveSystemSetup>();
        
        // Try to find the existing save system prefab
        GameObject saveSystemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SaveSystem.prefab");
        if (saveSystemPrefab != null)
        {
            setupComponent.saveSystemPrefab = saveSystemPrefab;
        }
        else
        {
            Debug.LogWarning("Save System prefab not found. Create it first using Tools > Save System > Create Save System Prefab");
        }
        
        Debug.Log("Save System Setup created in the current scene");
        
        // Select the created GameObject
        Selection.activeObject = saveSystemSetupObj;
    }
}
#endif 