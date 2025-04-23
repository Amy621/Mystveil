using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Editor utility to set up all save system adapter components on a player GameObject
/// </summary>
public class PlayerSaveAdapterSetup : MonoBehaviour
{
    [MenuItem("Tools/Save System/Setup Player Save Adapters")]
    public static void SetupPlayerAdapters()
    {
        // Find player GameObject
        GameObject player = Selection.activeGameObject;
        
        if (player == null)
        {
            Debug.LogError("No GameObject selected. Please select the player GameObject in the hierarchy.");
            return;
        }
        
        // Add all adapter components
        bool componentsAdded = false;
        
        if (player.GetComponent<PlayerHealth>() == null)
        {
            player.AddComponent<PlayerHealth>();
            componentsAdded = true;
        }
        
        if (player.GetComponent<PlayerMana>() == null)
        {
            player.AddComponent<PlayerMana>();
            componentsAdded = true;
        }
        
        if (player.GetComponent<PlayerLevel>() == null)
        {
            player.AddComponent<PlayerLevel>();
            componentsAdded = true;
        }
        
        if (player.GetComponent<PlayerAttributesAdapter>() == null)
        {
            player.AddComponent<PlayerAttributesAdapter>();
            componentsAdded = true;
        }
        
        if (player.GetComponent<PlayerCombatStatsAdapter>() == null)
        {
            player.AddComponent<PlayerCombatStatsAdapter>();
            componentsAdded = true;
        }
        
        // Add the new PlayerSaveLink component
        if (player.GetComponent<PlayerSaveLink>() == null)
        {
            player.AddComponent<PlayerSaveLink>();
            componentsAdded = true;
        }
        
        // Check for tag
        if (player.tag != "Player")
        {
            player.tag = "Player";
            Debug.Log("Set tag to 'Player'");
            componentsAdded = true;
        }
        
        if (componentsAdded)
        {
            Debug.Log("Save system adapter components added to " + player.name);
        }
        else
        {
            Debug.Log("All save system adapter components already exist on " + player.name);
        }
    }
}
#endif 