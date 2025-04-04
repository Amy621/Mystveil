using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoingToNextForestLv : MonoBehaviour
{
    public ChangeScene sceneChanger;
    
    // This method will be called when another object enters the collider's trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided is the player (or any other object you want to check)
        if (other.CompareTag("Player"))  // Ensure your player object has the "Player" tag
        {
           ChangeForest();
        }
    }

    public void ChangeForest() {
        Debug.Log("Changing forest to...");
        
        if (ForestBase.curLevel == ForestBase.numLevels) {
            Debug.Log("Back to town");
            MoveToScene("Town");
            ForestBase.onDestroy();
        } else if (ForestBase.curLevel + 1 == ForestBase.numLevels) {
            Debug.Log("Boss battle");
            ForestBase.curLevel++;
            ForestBase.onDestroy();
            ForestBase.onCreateLevel();
        } else {
            Debug.Log("Next level");
            ForestBase.curLevel++;
            ForestBase.onDestroy();
            ForestBase.onCreateBossLevel();
        }
    }

    public void MoveToScene(string sceneName)
    {
        // Load the scene based on the provided scene name
        SceneManager.LoadScene(sceneName);
    }
}
