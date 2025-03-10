using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    public static QuestTracker Instance;
    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();

    void Awake()
    {
        // Check if there's already an existing instance of QuestTracker
        if (Instance == null)
        {
            Instance = this; // Set this as the Singleton instance
            DontDestroyOnLoad(gameObject); // Optionally persist across scene loads
        }
        else
        {
            // Destroy this duplicate instance
            Destroy(gameObject);
        }
    }

    public void AddQuest(Quest newQuest)
    {
        if (!activeQuests.Exists(q => q.title == newQuest.title) && 
            !completedQuests.Exists(q => q.title == newQuest.title))
        {
            newQuest.isActive = true;
            activeQuests.Add(newQuest);
        }
    }

    public void CompleteQuest(string questTitle, GameObject collectedItem)
{
    Quest quest = activeQuests.Find(q => q.title == questTitle);
    if (quest != null && quest.requiredItem == collectedItem)
    {
        quest.isCompleted = true;
    }
}
}
