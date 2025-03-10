using TMPro; // Add this namespace
using UnityEngine;
using UnityEngine.UI;

public class QuestEntryUI : MonoBehaviour
{
    public TMP_Text titleText;      // Use TMP_Text instead of Text
    public TMP_Text descriptionText;
    public TMP_Text statusText;

    public void Initialize(Quest quest)
    {
        titleText.text = quest.title;
        descriptionText.text = quest.description;
        statusText.text = quest.isCompleted ? "Completed" : "In Progress";
    }
}