using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateDialogue : MonoBehaviour
{
    public DialogueTrigger dialogueTrigger;
    void Start()
    {
        if (dialogueTrigger == null)
        {
            Debug.LogError("DialogueTrigger not assigned to IntroDialogue!");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided is the player (or any other object you want to check)
        if (other.CompareTag("Player"))  // Ensure your player object has the "Player" tag
        {
            dialogueTrigger.TriggerDialogue("ask_about_gate");
        }

        Destroy(gameObject);
    }
}
