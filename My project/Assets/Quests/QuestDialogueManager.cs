using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

namespace LegacyQuests
{
    /// <summary>
    /// Acts as a bridge between Ink dialogue and the quest system, allowing
    /// objectives to be completed through conversation.
    /// </summary>
    public class QuestDialogueManager : MonoBehaviour
    {
        private DialogueManager dialogueManager;
        private LegacyQuests.QuestManager questManager;
        
        // External functions for Ink to call
        private Dictionary<string, Ink.Runtime.Object> externalFunctions;
        
        private void Awake()
        {
            dialogueManager = GetComponent<DialogueManager>();
            questManager = QuestManager.Instance;
            
            if (dialogueManager == null)
            {
                dialogueManager = FindObjectOfType<DialogueManager>();
                if (dialogueManager == null)
                {
                    Debug.LogError("[QuestDialogueManager] DialogueManager not found");
                    return;
                }
            }
            
            if (questManager == null)
            {
                Debug.LogError("[QuestDialogueManager] QuestManager not found");
                return;
            }
            
            // Bind external functions to call from Ink
            RegisterExternalFunctions();
        }
        
        private void RegisterExternalFunctions()
        {
            // Wait for story to be initialized
            if (dialogueManager.mainInkFile == null) return;
            
            // Get the story once it's initialized
            Story story = new Story(dialogueManager.mainInkFile.text);
            
            // Register quest functions
            story.BindExternalFunction("startQuest", (string questName) => {
                StartQuestByName(questName);
            });
            
            story.BindExternalFunction("completeObjective", (string questName, string objectiveDescription) => {
                CompleteObjectiveByName(questName, objectiveDescription);
            });
            
            story.BindExternalFunction("isQuestActive", (string questName) => {
                return IsQuestActive(questName);
            });
            
            story.BindExternalFunction("isQuestComplete", (string questName) => {
                return IsQuestComplete(questName);
            });
            
            story.BindExternalFunction("getObjectiveProgress", (string questName, string objectiveDescription) => {
                return GetObjectiveProgress(questName, objectiveDescription);
            });
            
            // Add more functions as needed
        }
        
        // Function to be called from Ink to start a quest
        private void StartQuestByName(string questName)
        {
            QuestData quest = FindQuestByName(questName);
            if (quest != null)
            {
                questManager.AcceptQuest(quest);
                Debug.Log($"[QuestDialogueManager] Started quest: {questName}");
            }
            else
            {
                Debug.LogWarning($"[QuestDialogueManager] Could not find quest with name: {questName}");
            }
        }
        
        // Function to be called from Ink to complete an objective
        private void CompleteObjectiveByName(string questName, string objectiveDescription)
        {
            questManager.UpdateObjective(questName, objectiveDescription, 1);
            Debug.Log($"[QuestDialogueManager] Advanced objective: {objectiveDescription} for quest: {questName}");
        }
        
        // Function to be called from Ink to check if a quest is active
        private bool IsQuestActive(string questName)
        {
            QuestData quest = FindQuestByName(questName);
            if (quest != null)
            {
                return questManager.HasQuest(questName);
            }
            return false;
        }
        
        // Function to be called from Ink to check if a quest is complete
        private bool IsQuestComplete(string questName)
        {
            QuestData quest = FindQuestByName(questName);
            if (quest != null)
            {
                return questManager.WasQuestCompleted(questName);
            }
            return false;
        }
        
        // Function to be called from Ink to get objective progress
        private int GetObjectiveProgress(string questName, string objectiveDescription)
        {
            QuestData quest = FindQuestByName(questName);
            if (quest != null)
            {
                // Find the objective index
                for (int i = 0; i < quest.objectives.Count; i++)
                {
                    if (quest.objectives[i].description == objectiveDescription)
                    {
                        QuestData activeQuest = questManager.GetQuest(questName);
                        if (activeQuest != null)
                        {
                            return activeQuest.objectives[i].currentAmount;
                        }
                    }
                }
            }
            return 0;
        }
        
        // Helper to find a quest by name
        private QuestData FindQuestByName(string questName)
        {
            QuestData[] allQuests = Resources.FindObjectsOfTypeAll<QuestData>();
            foreach (QuestData quest in allQuests)
            {
                if (quest.questName == questName)
                {
                    return quest;
                }
            }
            return null;
        }
    }
} 