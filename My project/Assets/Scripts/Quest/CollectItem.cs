using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    public string questTitle;

   void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        // Pass the collected item to the tracker
        QuestTracker.Instance.CompleteQuest(questTitle, gameObject);
        Destroy(gameObject);
    }
}
}