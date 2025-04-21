using System.Collections.Generic;
using UnityEngine;

namespace LegacyQuests
{
    [RequireComponent(typeof(Collider))]
    public class NPCQuestProvider : MonoBehaviour
    {
        [SerializeField] private List<QuestData> availableQuests = new List<QuestData>();
        [SerializeField] private string npcName = "NPC";
        [SerializeField] private float interactionDistance = 3f;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private GameObject interactionPrompt;

        private bool playerInRange = false;
        private Transform player;
        private QuestManager questManager;

        private void Start()
        {
            questManager = QuestManager.Instance;
            if (questManager == null)
            {
                Debug.LogError("QuestManager not found in scene. Add QuestManager to your scene.");
            }

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }

            // Initialize all quests
            foreach (QuestData quest in availableQuests)
            {
                quest.Initialize();
            }
        }

        private void Update()
        {
            if (playerInRange && Input.GetKeyDown(interactKey))
            {
                InteractWithNPC();
            }

            // Update interaction prompt position and visibility
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(playerInRange && HasAvailableQuests());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                player = other.transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                player = null;
            }
        }

        private void InteractWithNPC()
        {
            // Check if the player is looking at the NPC
            if (player != null)
            {
                Vector3 directionToNPC = transform.position - player.position;
                float distanceToNPC = directionToNPC.magnitude;

                if (distanceToNPC <= interactionDistance)
                {
                    OpenQuestUI();
                }
            }
        }

        private void OpenQuestUI()
        {
            // Check for available quests
            List<QuestData> availableQuestsToShow = GetAvailableQuests();
            
            if (availableQuestsToShow.Count > 0)
            {
                // Show quest UI with available quests
                // This would interface with your dialogue system
                Debug.Log(npcName + " has " + availableQuestsToShow.Count + " quests available.");
                
                // Example of how to connect with the dialogue system
                // You would customize this based on your actual dialogue system
                TriggerQuestDialogue(availableQuestsToShow[0]);
            }
            else
            {
                // Show generic dialogue - no quests available
                Debug.Log(npcName + " has no quests available.");
            }
        }

        private List<QuestData> GetAvailableQuests()
        {
            List<QuestData> availableQuestsToShow = new List<QuestData>();
            
            foreach (QuestData quest in availableQuests)
            {
                // Only show quests that aren't active or completed
                if (!quest.isActive && !quest.isCompleted)
                {
                    availableQuestsToShow.Add(quest);
                }
                // Show active quests that have all objectives completed for turn-in
                else if (quest.isActive && !quest.isCompleted && AreAllObjectivesCompleted(quest))
                {
                    availableQuestsToShow.Add(quest);
                }
            }
            
            return availableQuestsToShow;
        }

        private bool HasAvailableQuests()
        {
            return GetAvailableQuests().Count > 0;
        }

        private bool AreAllObjectivesCompleted(QuestData quest)
        {
            foreach (QuestObjective objective in quest.objectives)
            {
                if (!objective.isCompleted)
                {
                    return false;
                }
            }
            return true;
        }

        // Example of how to connect with the dialogue system
        private void TriggerQuestDialogue(QuestData quest)
        {
            // Here you would integrate with your dialogue system
            // For example:
            
            // If quest is active and all objectives are completed
            if (quest.isActive && AreAllObjectivesCompleted(quest))
            {
                // Show completion dialogue
                Debug.Log("NPC dialogue: You've completed my quest! Here's your reward.");
                
                // Complete the quest
                if (questManager != null)
                {
                    questManager.CompleteQuest(quest.questName);
                }
            }
            // If quest is not active
            else if (!quest.isActive)
            {
                // Show quest offer dialogue
                Debug.Log("NPC dialogue: I have a quest for you: " + quest.questName);
                
                // Start the quest when accepted
                if (questManager != null)
                {
                    questManager.AcceptQuest(quest);
                }
            }
        }

        // Public method to check if this NPC has a specific quest
        public bool HasQuest(string questName)
        {
            return availableQuests.Exists(q => q.questName == questName);
        }

        // Public method to check if this NPC has a completed quest ready for turn-in
        public bool HasCompletedQuestForTurnIn(string questName)
        {
            QuestData quest = availableQuests.Find(q => q.questName == questName);
            
            return quest != null && quest.isActive && !quest.isCompleted && AreAllObjectivesCompleted(quest);
        }
    }
} 