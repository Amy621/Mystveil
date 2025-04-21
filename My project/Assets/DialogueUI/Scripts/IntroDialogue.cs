using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialogue : MonoBehaviour
{
    public DialogueTrigger dialogueTrigger;

    void Start()
    {
        if (dialogueTrigger == null)
        {
            Debug.LogError("DialogueTrigger not assigned to IntroDialogue!");
            return;
        }

        // Trigger the intro dialogue by setting the 'current_interaction'
        // to "start", which will then divert to the 'intro' knot in your Ink file.
        dialogueTrigger.TriggerDialogue("intro");
    }
}

