using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoingToNextForestLv : MonoBehaviour
{
    public ChangeScene sceneChanger;
    public GenerateGrid gridGenerator;
    
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
        Debug.Log("Chaning forest to...");
        
        gridGenerator = FindObjectOfType(typeof(GenerateGrid)) as GenerateGrid;

        if (ForestBase.curLevel == ForestBase.numLevels) {
            Debug.Log("Back to town");
            MoveToScene("Town");
            ForestBase.onDestroy();
        } else if (ForestBase.curLevel + 1 == ForestBase.numLevels) {
            Debug.Log("Boss battle");
            ForestBase.curLevel++;
            GameObject.Find("Player").transform.position = ForestBase.playerSpawn;
            ForestBase.onDestroy();
            gridGenerator.GenerateBossLevel();
        } else {
            Debug.Log("Next level");
            ForestBase.curLevel++;
            GameObject.Find("Player").transform.position = ForestBase.playerSpawn;
            ForestBase.onDestroy();
            gridGenerator.GenerateLevel();
        }
    }

    public void MoveToScene(string sceneName)
    {
        // Load the scene based on the provided scene name
        SceneManager.LoadScene(sceneName);
    }
}
