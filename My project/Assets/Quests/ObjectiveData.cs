using UnityEngine;

[CreateAssetMenu(fileName = "NewObjective", menuName = "Quests/Objective")]
public class ObjectiveData : ScriptableObject
{
    [TextArea] public string description;      // e.g. "Collect 5 healing herbs"
    public int requiredAmount = 1;             // how many times to complete
}
