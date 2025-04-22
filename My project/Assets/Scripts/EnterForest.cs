using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterForest : MonoBehaviour
{
    // This method will be called when another object enters the collider's trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided is the player (or any other object you want to check)
        if (other.CompareTag("Player"))  // Ensure your player object has the "Player" tag
        {
            // Call the scene change method, passing the scene name
            MoveToScene("Forest");  // Load the "Forest" scene
        }
    }

    public void MoveToScene(string sceneName)
    {
        // Load the scene based on the provided scene name
        SceneManager.LoadScene(sceneName);
    }
}