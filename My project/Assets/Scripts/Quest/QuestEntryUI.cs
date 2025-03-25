using TMPro; // Add this namespace
using UnityEngine;
using UnityEngine.UI;

public class QuestEntryUI : MonoBehaviour
{
    [Header("Quest Info")]
    public TMP_Text titleText;      // Use TMP_Text instead of Text
    public TMP_Text descriptionText;
    public TMP_Text giverText;
    public TMP_Text statusText;
    
    [Header("Objectives")]
    public Transform objectivesContainer;
    public GameObject objectivePrefab;
    
    [Header("Rewards")]
    public Transform rewardsContainer;
    public GameObject rewardPrefab;

    public void Initialize(Quest quest)
    {
        titleText.text = quest.title;
        descriptionText.text = quest.description;
        giverText.text = $"Given by: {quest.giver}";
        statusText.text = quest.isCompleted ? "Completed" : "In Progress";

        // Clear existing objectives
        foreach (Transform child in objectivesContainer)
        {
            Destroy(child.gameObject);
        }

        // Add objectives
        foreach (QuestObjective objective in quest.objectives)
        {
            GameObject obj = Instantiate(objectivePrefab, objectivesContainer);
            TMP_Text objText = obj.GetComponent<TMP_Text>();
            objText.text = $"• {objective.description} ({objective.current}/{objective.required})";
            
            // Gray out completed objectives
            if (objective.isCompleted)
            {
                objText.color = Color.gray;
            }
        }

        // Clear existing rewards
        foreach (Transform child in rewardsContainer)
        {
            Destroy(child.gameObject);
        }

        // Add rewards
        foreach (QuestReward reward in quest.rewards)
        {
            GameObject rewardObj = Instantiate(rewardPrefab, rewardsContainer);
            rewardObj.GetComponentInChildren<Image>().sprite = reward.itemIcon;
            rewardObj.GetComponentInChildren<TMP_Text>().text = $"{reward.quantity}x {reward.itemName}";
        }
    }
}