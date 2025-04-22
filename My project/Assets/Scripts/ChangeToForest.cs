using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToForest : MonoBehaviour
{
    public void MoveToScene() {
    string currentScene = SceneManager.GetActiveScene().name;
    Debug.Log($"Current Scene: {currentScene}");
    Debug.Log("Attempting to load scene named 'Forest'");
    SceneManager.LoadScene("Forest");
    }
}