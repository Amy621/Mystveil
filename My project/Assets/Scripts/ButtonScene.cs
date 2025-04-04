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
        yourButton.onClick.AddListener(() => MoveToScene(1));
    }

    public void MoveToScene(int sceneID) {
        SceneManager.LoadScene(sceneID);
    }
}
