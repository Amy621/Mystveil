using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalterInteraction : MonoBehaviour
{
    public DialogueTrigger dialogueTrigger;
    public GameObject interactIcon;
    private bool playerInRange;

    void Start()
    {
         if (dialogueTrigger == null)
        {
            Debug.LogError("DialogueTrigger not assigned to IntroDialogue!");
        }
    }

     void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
           dialogueTrigger.TriggerDialogue("walter_intro");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactIcon.SetActive(true);
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactIcon.SetActive(false);
            playerInRange = false;
        }
    }
}
