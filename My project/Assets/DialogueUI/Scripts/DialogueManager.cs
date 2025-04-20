using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System;

public class DialogueManager : MonoBehaviour
{
    public TextAsset mainInkFile;

    public int player_charisma;
    private Story mainStory;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        InitializeMain();
    }

    void InitializeMain()
    {
        if (mainInkFile != null)
        {
            mainStory = new Story(mainInkFile.text);
            player_charisma = (int) mainStory.variablesState["player_charisma"];
        }
        else
        {
            Debug.LogError("Main Ink file not assigned in the Inspector!");
        }
    }

    public Story GetMainStory()
    {
        return mainStory;
    }

    public void UpdateGlobalVariable()
    {
        player_charisma = (int) mainStory.variablesState["player_charisma"];
        Debug.Log($"Ink variable updated to: {player_charisma}");
    }
}