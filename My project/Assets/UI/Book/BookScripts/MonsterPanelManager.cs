using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MonsterPanelManager : MonoBehaviour
{
    public static MonsterPanelManager Instance;

    [Header("Monster Grid")]
    public List<BookMonsterEntry> allMonsters = new List<BookMonsterEntry>();
    public GameObject monsterEntryPrefab;
    public Transform monsterGridContent;
    public GridLayoutGroup gridLayout;

    [Header("Detail View")]
    public Image detailMonsterImage;
    public TMP_Text detailMonsterName;
    public TMP_Text detailDescription;
    public GameObject noMonsterSelectedText;

    private MonsterEntryUI currentlySelected;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Set up grid layout
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 5; // 5 columns as shown in the image
        }
    }

    void Start()
    {
        InitializeMonsterList();
        ClearDetailView();
    }

    void InitializeMonsterList()
    {
        // Clear existing entries
        foreach (Transform child in monsterGridContent)
        {
            Destroy(child.gameObject);
        }

        // Create entries for all monsters
        foreach (BookMonsterEntry monster in allMonsters)
        {
            CreateMonsterEntry(monster);
        }
    }

    void CreateMonsterEntry(BookMonsterEntry monster)
    {
        GameObject entry = Instantiate(monsterEntryPrefab, monsterGridContent);
        MonsterEntryUI entryUI = entry.GetComponent<MonsterEntryUI>();
        if (entryUI != null)
        {
            entryUI.Initialize(monster);
        }
    }

    public void SelectMonster(BookMonsterEntry monster)
    {
        // Deselect previous
        if (currentlySelected != null)
        {
            currentlySelected.SetSelected(false);
        }

        // Update detail view
        if (monster != null && monster.isDiscovered)
        {
            detailMonsterImage.sprite = monster.monsterIcon;
            detailMonsterName.text = monster.monsterName;
            detailDescription.text = monster.description;
            
            noMonsterSelectedText.SetActive(false);
            detailMonsterImage.gameObject.SetActive(true);
            detailMonsterName.gameObject.SetActive(true);
            detailDescription.gameObject.SetActive(true);

            // Find and select the new entry
            MonsterEntryUI[] allEntries = monsterGridContent.GetComponentsInChildren<MonsterEntryUI>();
            foreach (MonsterEntryUI entry in allEntries)
            {
                if (entry.MonsterData == monster)
                {
                    entry.SetSelected(true);
                    currentlySelected = entry;
                    break;
                }
            }
        }
        else
        {
            ClearDetailView();
        }
    }

    void ClearDetailView()
    {
        noMonsterSelectedText.SetActive(true);
        detailMonsterImage.gameObject.SetActive(false);
        detailMonsterName.gameObject.SetActive(false);
        detailDescription.gameObject.SetActive(false);
    }

    public void DiscoverMonster(string monsterName)
    {
        BookMonsterEntry monster = allMonsters.Find(m => m.monsterName == monsterName);
        if (monster != null && !monster.isDiscovered)
        {
            monster.isDiscovered = true;
            UpdateMonsterList();
        }
    }

    void UpdateMonsterList()
    {
        // Refresh all entries
        MonsterEntryUI[] entries = monsterGridContent.GetComponentsInChildren<MonsterEntryUI>();
        foreach (MonsterEntryUI entry in entries)
        {
            entry.Initialize(entry.MonsterData);
        }
    }
} 