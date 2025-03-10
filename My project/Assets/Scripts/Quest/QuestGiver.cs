using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public Quest quest;
    public GameObject interactIcon;
    private bool playerInRange;
    
    // Reference to the AudioSource component
    public AudioSource questAudio;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Add the quest to the tracker
            QuestTracker.Instance.AddQuest(quest);

            // Play the audio
            if (questAudio != null)
            {
                questAudio.Play();
            }

            // Hide the interact icon
            interactIcon.SetActive(false);
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
