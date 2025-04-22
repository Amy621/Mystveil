using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToFinalBoss : MonoBehaviour
{
    public void MoveToScene() {
    string currentScene = SceneManager.GetActiveScene().name;
    Debug.Log($"Current Scene: {currentScene}");
    Debug.Log("Attempting to load scene named 'FinalBoss'");
    SceneManager.LoadScene("FinalBoss");
    }
}
