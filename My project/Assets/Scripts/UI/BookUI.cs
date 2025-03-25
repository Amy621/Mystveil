using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BookUI : MonoBehaviour
{
    public static BookUI Instance;
    
    [Header("Book UI Elements")]
    public GameObject bookIcon; // The clickable icon
    public GameObject bookPanel; // The main book panel
    public GameObject tabsParent; // Parent object containing all tab buttons
    
    [Header("Tab Panels")]
    public GameObject introductionPanel;
    public GameObject lorePanel;
    public GameObject monstersPanel;
    public GameObject spellsPanel;
    public GameObject itemsPanel;
    public GameObject questsPanel;
    
    [Header("Content Templates")]
    public GameObject monsterEntryPrefab;
    public GameObject itemEntryPrefab;
    public GameObject questEntryPrefab;
    public GameObject spellEntryPrefab;
    
    [Header("Content Parents")]
    public Transform monstersContent;
    public Transform itemsContent;
    public Transform questsContent;
    public Transform spellsContent;
    
    private Dictionary<string, bool> discoveredMonsters = new Dictionary<string, bool>();
    private Dictionary<string, bool> discoveredItems = new Dictionary<string, bool>();
    
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
    }
    
    void Start()
    {
        // Initialize the book as closed
        bookPanel.SetActive(false);
        bookIcon.SetActive(true);
        
        // Add click listener to the book icon
        bookIcon.GetComponent<Button>().onClick.AddListener(ToggleBook);
    }
    
    public void ToggleBook()
    {
        bookPanel.SetActive(!bookPanel.activeSelf);
        
        if (bookPanel.activeSelf)
        {
            UpdateAllTabs();
        }
    }
    
    public void SwitchTab(string tabName)
    {
        introductionPanel.SetActive(tabName == "Introduction");
        lorePanel.SetActive(tabName == "Lore");
        monstersPanel.SetActive(tabName == "Monsters");
        spellsPanel.SetActive(tabName == "Spells");
        itemsPanel.SetActive(tabName == "Items");
        questsPanel.SetActive(tabName == "Quests");
    }
    
    public void DiscoverMonster(string monsterName, Sprite monsterSprite, string description)
    {
        if (!discoveredMonsters.ContainsKey(monsterName))
        {
            discoveredMonsters[monsterName] = true;
            CreateMonsterEntry(monsterName, monsterSprite, description);
        }
    }
    
    public void DiscoverItem(string itemName, Sprite itemSprite, string description)
    {
        if (!discoveredItems.ContainsKey(itemName))
        {
            discoveredItems[itemName] = true;
            CreateItemEntry(itemName, itemSprite, description);
        }
    }
    
    private void CreateMonsterEntry(string name, Sprite sprite, string description)
    {
        GameObject entry = Instantiate(monsterEntryPrefab, monstersContent);
        entry.GetComponentInChildren<Image>().sprite = sprite;
        entry.GetComponentInChildren<TMP_Text>().text = name;
        entry.GetComponent<TooltipTrigger>().tooltipText = description;
    }
    
    private void CreateItemEntry(string name, Sprite sprite, string description)
    {
        GameObject entry = Instantiate(itemEntryPrefab, itemsContent);
        entry.GetComponentInChildren<Image>().sprite = sprite;
        entry.GetComponentInChildren<TMP_Text>().text = name;
        entry.GetComponent<TooltipTrigger>().tooltipText = description;
    }
    
    private void UpdateAllTabs()
    {
        // Update quests
        foreach (Transform child in questsContent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Quest quest in QuestTracker.Instance.activeQuests)
        {
            GameObject entry = Instantiate(questEntryPrefab, questsContent);
            QuestEntryUI questUI = entry.GetComponent<QuestEntryUI>();
            if (questUI != null)
            {
                questUI.Initialize(quest);
            }
        }
        
        // Update other tabs as needed
    }
} 