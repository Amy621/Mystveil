using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonScene : MonoBehaviour
{
    public Button yourButton;

    void Start()
    {
        // supposed to be the intro level which is 1 but this is for testing purposes!
        yourButton.onClick.AddListener(() => MoveToScene(1));
    }

    public void MoveToScene(int sceneID) {
        SceneManager.LoadScene(sceneID);
    }
}
