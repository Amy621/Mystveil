using UnityEngine;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    public static BookManager Instance;

    [Header("Book Panels")]
    public GameObject introductionPanel;
    public GameObject lorePanel;
    public GameObject monstersPanel;
    public GameObject spellsPanel;
    public GameObject itemsPanel;
    public GameObject questsPanel;

    [Header("Tab Buttons")]
    public Button introductionButton;
    public Button loreButton;
    public Button monstersButton;
    public Button spellsButton;
    public Button itemsButton;
    public Button questsButton;

    [Header("Settings")]
    public bool startWithAllTabsLocked = true;
    public bool introductionUnlockedByDefault = true;

    private void Awake()
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

    private void Start()
    {
        // Set up button listeners
        introductionButton.onClick.AddListener(() => OpenSection("Introduction"));
        loreButton.onClick.AddListener(() => OpenSection("Lore"));
        monstersButton.onClick.AddListener(() => OpenSection("Monsters"));
        spellsButton.onClick.AddListener(() => OpenSection("Spells"));
        itemsButton.onClick.AddListener(() => OpenSection("Items"));
        questsButton.onClick.AddListener(() => OpenSection("Quests"));

        // Initialize tab states
        if (startWithAllTabsLocked)
        {
            // Lock all tabs except introduction if specified
            SetTabLocked(loreButton, true);
            SetTabLocked(monstersButton, true);
            SetTabLocked(spellsButton, true);
            SetTabLocked(itemsButton, true);
            SetTabLocked(questsButton, true);

            if (introductionUnlockedByDefault)
            {
                SetTabLocked(introductionButton, false);
            }
            else
            {
                SetTabLocked(introductionButton, true);
            }
        }

        // Start with introduction panel if it's unlocked
        if (introductionUnlockedByDefault)
        {
            OpenSection("Introduction");
        }
        else
        {
            HideAllSections();
        }
    }

    public void OpenSection(string sectionName)
    {
        // Hide all panels first
        HideAllSections();

        // Show the selected panel
        switch (sectionName)
        {
            case "Introduction":
                introductionPanel.SetActive(true);
                break;
            case "Lore":
                lorePanel.SetActive(true);
                break;
            case "Monsters":
                monstersPanel.SetActive(true);
                break;
            case "Spells":
                spellsPanel.SetActive(true);
                break;
            case "Items":
                itemsPanel.SetActive(true);
                break;
            case "Quests":
                questsPanel.SetActive(true);
                break;
        }
    }

    private void HideAllSections()
    {
        introductionPanel.SetActive(false);
        lorePanel.SetActive(false);
        monstersPanel.SetActive(false);
        spellsPanel.SetActive(false);
        itemsPanel.SetActive(false);
        questsPanel.SetActive(false);
    }

    public void UnlockTab(string tabName)
    {
        switch (tabName)
        {
            case "Introduction":
                SetTabLocked(introductionButton, false);
                break;
            case "Lore":
                SetTabLocked(loreButton, false);
                break;
            case "Monsters":
                SetTabLocked(monstersButton, false);
                break;
            case "Spells":
                SetTabLocked(spellsButton, false);
                break;
            case "Items":
                SetTabLocked(itemsButton, false);
                break;
            case "Quests":
                SetTabLocked(questsButton, false);
                break;
        }
    }

    private void SetTabLocked(Button button, bool locked)
    {
        button.interactable = !locked;
        
        // Optional: Change the appearance of locked buttons
        if (locked)
        {
            // Make the button appear grayed out or add a lock icon
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            button.colors = colors;
        }
        else
        {
            // Restore normal appearance
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            button.colors = colors;
        }
    }
} 