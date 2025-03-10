using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string title;
    public string description;
    public bool isCompleted;
    public bool isActive;
    public GameObject requiredItem;
}
