using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyQuests
{
    public class QuestProgressManager : MonoBehaviour
    {
        public static QuestProgressManager Instance { get; private set; }

        [Tooltip("Drag in all your QuestData assets here")]
        [SerializeField] private QuestData[] availableQuests;

        // runtime map: QuestData → current counts per objective
        private Dictionary<QuestData, int[]> questProgress = new();

        // event for quest state changes
        public delegate void QuestStateChangedDelegate(QuestData quest, bool isCompleted);
        public event QuestStateChangedDelegate OnQuestStateChanged;

        // where we save/load the JSON
        private string saveFilePath;

        // ---- Serializable helpers ----
        [Serializable]
        private class QuestProgressEntry
        {
            public string questName;
            public int[] progress;
        }

        [Serializable]
        private class QuestProgressData
        {
            public List<QuestProgressEntry> quests = new();
        }
        // -------------------------------

        private void Awake()
        {
            // singleton + persist
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                saveFilePath = Path.Combine(Application.persistentDataPath, "questProgress.json");
                LoadAllProgressFromFile();
            }
            else Destroy(gameObject);
        }

        /// <summary>
        /// Call this to activate a new quest.
        /// Won't overwrite if it was already loaded from file.
        /// </summary>
        public void StartQuest(QuestData quest)
        {
            if (!questProgress.ContainsKey(quest))
            {
                questProgress[quest] = new int[quest.objectives.Count];
                Debug.Log($"[QuestProgressManager] Started quest '{quest.questName}'");
                SaveAllProgressToFile();
                OnQuestStateChanged?.Invoke(quest, false);
            }
        }

        /// <summary>
        /// Call this (e.g. when player picks up an item) to advance a specific objective.
        /// </summary>
        public void AdvanceObjective(QuestData quest, int objectiveIndex)
        {
            if (!questProgress.ContainsKey(quest)) return;

            int[] prog = questProgress[quest];
            if (objectiveIndex >= 0 && objectiveIndex < prog.Length && 
                prog[objectiveIndex] < quest.objectives[objectiveIndex].requiredAmount)
            {
                prog[objectiveIndex]++;
                Debug.Log($"[QuestProgressManager] '{quest.questName}' obj #{objectiveIndex} -> {prog[objectiveIndex]}/{quest.objectives[objectiveIndex].requiredAmount}");
                SaveAllProgressToFile();
                
                // Check if quest is now complete and trigger event if so
                bool isComplete = IsQuestComplete(quest);
                if (isComplete)
                {
                    OnQuestStateChanged?.Invoke(quest, true);
                }
            }
        }

        /// <summary>
        /// Returns true if every objective has reached its required amount.
        /// </summary>
        public bool IsQuestComplete(QuestData quest)
        {
            if (!questProgress.ContainsKey(quest)) return false;
            int[] prog = questProgress[quest];
            for (int i = 0; i < prog.Length; i++)
                if (prog[i] < quest.objectives[i].requiredAmount)
                    return false;
            return true;
        }
        
        /// <summary>
        /// Returns true if the quest has been started but is not complete.
        /// </summary>
        public bool IsQuestActive(QuestData quest)
        {
            return questProgress.ContainsKey(quest) && !IsQuestComplete(quest);
        }
        
        /// <summary>
        /// Get the current progress of a specific objective for a quest.
        /// </summary>
        /// <returns>Current progress (0 if quest not active)</returns>
        public int GetObjectiveProgress(QuestData quest, int objectiveIndex)
        {
            if (!questProgress.ContainsKey(quest)) return 0;
            
            int[] prog = questProgress[quest];
            if (objectiveIndex >= 0 && objectiveIndex < prog.Length)
            {
                return prog[objectiveIndex];
            }
            
            return 0;
        }
        
        /// <summary>
        /// Advances an objective for a quest by name and objective description.
        /// Useful for advancing objectives from dialogue or item pickups.
        /// </summary>
        public void AdvanceObjectiveByName(string questName, string objectiveDescription)
        {
            foreach (QuestData quest in questProgress.Keys)
            {
                if (quest.questName == questName)
                {
                    for (int i = 0; i < quest.objectives.Count; i++)
                    {
                        if (quest.objectives[i].description == objectiveDescription)
                        {
                            AdvanceObjective(quest, i);
                            return;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Get all active quests that aren't completed.
        /// </summary>
        public List<QuestData> GetActiveQuests()
        {
            List<QuestData> activeQuests = new List<QuestData>();
            
            foreach (var quest in questProgress.Keys)
            {
                if (!IsQuestComplete(quest))
                {
                    activeQuests.Add(quest);
                }
            }
            
            return activeQuests;
        }
        
        /// <summary>
        /// Get all completed quests.
        /// </summary>
        public List<QuestData> GetCompletedQuests()
        {
            List<QuestData> completedQuests = new List<QuestData>();
            
            foreach (var quest in questProgress.Keys)
            {
                if (IsQuestComplete(quest))
                {
                    completedQuests.Add(quest);
                }
            }
            
            return completedQuests;
        }

        // Write out the entire questProgress dictionary to JSON
        private void SaveAllProgressToFile()
        {
            var container = new QuestProgressData();
            foreach (var kv in questProgress)
            {
                container.quests.Add(new QuestProgressEntry {
                    questName = kv.Key.questName,
                    progress  = kv.Value
                });
            }

            try
            {
                string json = JsonUtility.ToJson(container, true);
                File.WriteAllText(saveFilePath, json);
                Debug.Log($"[QuestProgressManager] Saved progress to '{saveFilePath}'");
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuestProgressManager] Failed saving JSON: {e}");
            }
        }

        // Load any prior saves, matching by questName to your availableQuests list
        private void LoadAllProgressFromFile()
        {
            if (!File.Exists(saveFilePath)) return;

            try
            {
                string json = File.ReadAllText(saveFilePath);
                var container = JsonUtility.FromJson<QuestProgressData>(json);

                foreach (var entry in container.quests)
                {
                    var quest = Array.Find(availableQuests, q => q.questName == entry.questName);
                    if (quest != null)
                    {
                        // resize/copy progress safely
                        int len = quest.objectives.Count;
                        int[] prog = new int[len];
                        for (int i = 0; i < len; i++)
                            prog[i] = i < entry.progress.Length ? entry.progress[i] : 0;
                        questProgress[quest] = prog;
                        Debug.Log($"[QuestProgressManager] Loaded '{quest.questName}' ({string.Join(",", prog)})");
                    }
                    else
                    {
                        Debug.LogWarning($"[QuestProgressManager] JSON had unknown quest '{entry.questName}'");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuestProgressManager] Failed loading JSON: {e}");
            }
        }

        // (Optional) ensure final save on quit
        private void OnApplicationQuit()
        {
            SaveAllProgressToFile();
        }
    }
}
