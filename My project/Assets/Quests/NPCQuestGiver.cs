using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyQuests 
{
    public class NPCQuestGiver : MonoBehaviour
    {
        [Header("Quest Configuration")]
        [Tooltip("Assign quest data for this NPC to offer")]
        [SerializeField] private QuestData[] availableQuests;
        
        [Header("Dialogue Settings")]
        [Tooltip("Dialogue interaction name for quest acceptance")]
        [SerializeField] private string questAcceptDialogue = "quest_accept";
        [Tooltip("Dialogue interaction name for quest in progress")]
        [SerializeField] private string questActiveDialogue = "quest_active";
        [Tooltip("Dialogue interaction name for quest completion")]
        [SerializeField] private string questCompleteDialogue = "quest_complete";
        [Tooltip("Dialogue interaction name for when all quests are done")]
        [SerializeField] private string allQuestsCompleteDialogue = "all_quests_complete";
        
        [Header("Interaction Settings")]
        [SerializeField] private float interactionDistance = 3f;
        [SerializeField] private GameObject interactionIndicator;
        
        private DialogueManager dialogueManager;
        private QuestManager questManager;
        private bool playerInRange = false;
        private int currentQuestIndex = 0;
        
        private void Start()
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
            questManager = QuestManager.Instance;
            
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(false);
            }
            
            if (dialogueManager == null)
            {
                Debug.LogError($"[NPCQuestGiver] DialogueManager not found in scene for NPC {gameObject.name}");
            }
            
            if (questManager == null)
            {
                Debug.LogError($"[NPCQuestGiver] QuestManager not found in scene for NPC {gameObject.name}");
            }
        }
        
        private void Update()
        {
            CheckPlayerDistance();
            
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                StartQuestInteraction();
            }
        }
        
        private void CheckPlayerDistance()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                bool wasInRange = playerInRange;
                playerInRange = distance <= interactionDistance;
                
                // Only update if the state changed
                if (wasInRange != playerInRange && interactionIndicator != null)
                {
                    interactionIndicator.SetActive(playerInRange);
                }
            }
        }
        
        private void StartQuestInteraction()
        {
            if (dialogueManager == null || questManager == null) return;
            
            // No quests available
            if (availableQuests.Length == 0)
            {
                dialogueManager.StartDialogue(allQuestsCompleteDialogue);
                return;
            }
            
            // Find the current active quest
            QuestData currentQuest = GetCurrentQuest();
            
            if (currentQuest == null)
            {
                // All quests completed
                dialogueManager.StartDialogue(allQuestsCompleteDialogue);
                return;
            }
            
            // Check if the current quest is completed
            if (questManager.WasQuestCompleted(currentQuest.questName))
            {
                // Set quest completion variable for ink
                dialogueManager.SetVariable("quest_name", currentQuest.questName);
                dialogueManager.StartDialogue(questCompleteDialogue);
                
                // Advance to next quest for next interaction
                currentQuestIndex++;
                return;
            }
            
            // Check if the quest is already active
            if (questManager.HasQuest(currentQuest.questName))
            {
                // Set quest status variables for ink
                dialogueManager.SetVariable("quest_name", currentQuest.questName);
                SetQuestProgressVariables(currentQuest);
                
                // Start in-progress dialogue
                dialogueManager.StartDialogue(questActiveDialogue);
            }
            else
            {
                // Offer new quest
                dialogueManager.SetVariable("quest_name", currentQuest.questName);
                dialogueManager.SetVariable("quest_description", currentQuest.description);
                
                // Register for dialogue completion to give the quest
                dialogueManager.OnDialogueEnded += GiveCurrentQuest;
                
                // Start acceptance dialogue
                dialogueManager.StartDialogue(questAcceptDialogue);
            }
        }
        
        private void GiveCurrentQuest()
        {
            // Unregister to prevent multiple calls
            dialogueManager.OnDialogueEnded -= GiveCurrentQuest;
            
            QuestData currentQuest = GetCurrentQuest();
            if (currentQuest != null)
            {
                questManager.AcceptQuest(currentQuest);
                Debug.Log($"[NPCQuestGiver] {gameObject.name} gave quest: {currentQuest.questName}");
            }
        }
        
        private QuestData GetCurrentQuest()
        {
            // Find the first quest that isn't completed yet
            for (int i = currentQuestIndex; i < availableQuests.Length; i++)
            {
                QuestData quest = availableQuests[i];
                if (!questManager.WasQuestCompleted(quest.questName))
                {
                    currentQuestIndex = i;
                    return quest;
                }
            }
            
            return null; // All quests completed
        }
        
        private void SetQuestProgressVariables(QuestData quest)
        {
            // Set quest progress variables for the ink script to use
            if (dialogueManager != null)
            {
                QuestData activeQuest = questManager.GetQuest(quest.questName);
                if (activeQuest != null)
                {
                    for (int i = 0; i < activeQuest.objectives.Count; i++)
                    {
                        int progress = activeQuest.objectives[i].currentAmount;
                        int required = activeQuest.objectives[i].requiredAmount;
                        
                        dialogueManager.SetVariable($"objective_{i}_current", progress);
                        dialogueManager.SetVariable($"objective_{i}_required", required);
                        dialogueManager.SetVariable($"objective_{i}_desc", activeQuest.objectives[i].description);
                    }
                    
                    dialogueManager.SetVariable("objective_count", activeQuest.objectives.Count);
                }
            }
        }
    }
} 