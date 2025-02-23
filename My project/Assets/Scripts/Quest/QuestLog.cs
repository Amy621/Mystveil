using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Add TextMeshPro namespace

public class QuestLog : MonoBehaviour
{
    public GameObject questLogList; // Assign the root UI panel
    public Transform questListContent; // Content parent for entries
    public GameObject questEntryPrefab; // Assign your quest entry prefab

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleQuestLog();
        }
    }

    void ToggleQuestLog()
    {
        bool shouldOpen = !questLogList.activeSelf;
        questLogList.SetActive(shouldOpen);

        if (shouldOpen)
        {
            UpdateQuestList();
        }
    }

    public void UpdateQuestList()
    {
        // Clear old entries
        foreach (Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        // Safety checks
        if (questEntryPrefab == null)
        {
            Debug.LogError("QuestEntryPrefab is not assigned!");
            return;
        }

        // Populate new entries
        foreach (Quest quest in QuestTracker.Instance.activeQuests)
        {
            CreateQuestEntry(quest);
        }
    }

    void CreateQuestEntry(Quest quest)
    {
        GameObject entry = Instantiate(questEntryPrefab, questListContent);
        QuestEntryUI entryUI = entry.GetComponent<QuestEntryUI>();

        if (entryUI != null)
        {
            entryUI.Initialize(quest);
        }
        else
        {
            Debug.LogError("QuestEntryPrefab is missing QuestEntryUI component!");
        }
    }
}