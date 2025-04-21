using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class BookMonsterEntry
{
    public string monsterName;
    public string description;
    public Sprite monsterIcon;
    public bool isDiscovered;
}

public class MonsterEntryUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Square Display")]
    public Image monsterIcon;
    public Image frameImage;
    public Color discoveredColor = Color.white;
    public Color undiscoveredColor = Color.gray;
    
    private BookMonsterEntry monsterData;
    private bool isSelected;

    // Public property to access monster data
    public BookMonsterEntry MonsterData => monsterData;

    public void Initialize(BookMonsterEntry monster)
    {
        monsterData = monster;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (monsterData.isDiscovered)
        {
            monsterIcon.sprite = monsterData.monsterIcon;
            monsterIcon.color = discoveredColor;
            frameImage.color = discoveredColor;
        }
        else
        {
            monsterIcon.sprite = null; // Or use a question mark sprite
            monsterIcon.color = undiscoveredColor;
            frameImage.color = undiscoveredColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (monsterData.isDiscovered)
        {
            MonsterPanelManager.Instance.SelectMonster(monsterData);
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        // Add visual feedback for selection (e.g., highlight frame)
        frameImage.color = selected ? Color.yellow : (monsterData.isDiscovered ? discoveredColor : undiscoveredColor);
    }
} 