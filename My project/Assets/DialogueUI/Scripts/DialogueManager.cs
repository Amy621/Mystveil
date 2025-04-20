using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextAsset mainInkFile;
    private Story currentStory;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        InitializeStory();
    }

    void InitializeStory()
    {
        if (mainInkFile != null)
        {
            currentStory = new Story(mainInkFile.text);
        }
        else
        {
            Debug.LogError("Main Ink file not assigned in the Inspector!");
        }
    }

    public void StartDialogue(string interactionToStart)
    {
        if (currentStory == null)
        {
            Debug.LogError("Ink story is not initialized. Make sure the main Ink file is assigned.");
            return;
        }
        Debug.Log(interactionToStart);
        // Set the global variable to control the starting point
        currentStory.variablesState["current_interaction"] = interactionToStart;

        // Divert to the starting knot based on the variable (optional, can also be handled in Ink directly)
        currentStory.ChoosePathString(interactionToStart);

        // Signal that dialogue has started (you might want to handle UI activation here or in GameController)
        if (OnDialogueStarted != null)
        {
            OnDialogueStarted.Invoke(currentStory); // Pass the story object
        }
    }

    public bool CanContinueStory()
    {
        return currentStory != null && currentStory.canContinue;
    }

    public string GetNextStoryLine()
    {
        if (CanContinueStory())
        {
            return currentStory.Continue();
        }
        return null;
    }

    public List<Choice> GetCurrentChoices()
    {
        if (currentStory != null)
        {
            return currentStory.currentChoices;
        }
        return new List<Choice>();
    }

    public void MakeChoice(int choiceIndex)
    {
        if (currentStory != null && currentStory.currentChoices.Count > choiceIndex)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
        }
        else
        {
            Debug.LogError($"Invalid choice index: {choiceIndex}");
        }
    }

    public void SetVariable(string variableName, object value)
    {
        if (currentStory != null)
        {
            currentStory.variablesState[variableName] = value;
        }
        else
        {
            Debug.LogError("Ink story is not initialized.");
        }
    }

    public object GetVariable(string variableName)
    {
        return currentStory.variablesState[variableName];
    }

    public event System.Action<Story> OnDialogueStarted;
    public event System.Action OnDialogueEnded;

    public void EndDialogue()
    {
        currentStory = null; // Clean up the story
        OnDialogueEnded?.Invoke();
        InitializeStory(); // Re-initialize for the next dialogue
    }
}