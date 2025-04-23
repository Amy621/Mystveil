using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToTown : MonoBehaviour
{
    public void MoveToScene() {
    string currentScene = SceneManager.GetActiveScene().name;
    Debug.Log($"Current Scene: {currentScene}");
    Debug.Log("Attempting to load scene named 'Town 2'");
    SceneManager.LoadScene("Town 2");
    }
}
