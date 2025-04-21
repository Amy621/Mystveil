using UnityEngine;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    [Header("Book UI")]
    [SerializeField] private GameObject bookCanvas;
    [SerializeField] private KeyCode bookToggleKey = KeyCode.B;

    [Header("Book Panels")]
    [SerializeField] private GameObject introductionPanel;
    [SerializeField] private GameObject lorePanel;
    [SerializeField] private GameObject monstersPanel;
    [SerializeField] private GameObject spellsPanel;
    [SerializeField] private GameObject itemsPanel;
    [SerializeField] private GameObject questsPanel;

    [Header("Tab Buttons")]
    [SerializeField] private Button introductionButton;
    [SerializeField] private Button loreButton;
    [SerializeField] private Button monstersButton;
    [SerializeField] private Button spellsButton;
    [SerializeField] private Button itemsButton;
    [SerializeField] private Button questsButton;

    [Header("Settings")]
    [SerializeField] private bool startWithAllTabsLocked = true;
    [SerializeField] private bool introductionUnlockedByDefault = true;
    
    // Book state
    private bool isBookOpen = false;
    private string lastOpenSection = "Introduction";

    // Property accessors for BookSystem
    public KeyCode BookToggleKey { get => bookToggleKey; set => bookToggleKey = value; }
    public bool StartWithAllTabsLocked { get => startWithAllTabsLocked; set => startWithAllTabsLocked = value; }
    public bool IntroductionUnlockedByDefault { get => introductionUnlockedByDefault; set => introductionUnlockedByDefault = value; }
    
    // Panel accessors
    public GameObject IntroductionPanel { get => introductionPanel; }
    public GameObject LorePanel { get => lorePanel; }
    public GameObject MonstersPanel { get => monstersPanel; }
    public GameObject SpellsPanel { get => spellsPanel; }
    public GameObject ItemsPanel { get => itemsPanel; }
    public GameObject QuestsPanel { get => questsPanel; }

    private void Awake()
    {
        // Automatically find the canvas reference if it's not set
        if (bookCanvas == null)
        {
            // First try to get the canvas component on this GameObject
            Canvas canvasComponent = GetComponent<Canvas>();
            if (canvasComponent != null)
            {
                bookCanvas = gameObject;
                Debug.Log("BookManager: Automatically set canvas reference to this GameObject");
            }
            else
            {
                // If there's no Canvas component on this GameObject, use the parent
                bookCanvas = transform.parent?.gameObject;
                Debug.Log("BookManager: Automatically set canvas reference to parent GameObject");
            }
            
            if (bookCanvas == null)
            {
                Debug.LogError("BookManager: Could not find a canvas reference automatically");
            }
        }
    }

    private void Start()
    {
        // Set up button listeners
        SetupButtonListeners();

        // Initialize tab states
        InitializeTabStates();

        // Start with book closed
        CloseBook();
        
        Debug.Log("BookManager started - Press " + bookToggleKey + " to toggle the book");
    }
    
    private void SetupButtonListeners()
    {
        if (introductionButton != null)
            introductionButton.onClick.AddListener(() => OpenSection("Introduction"));
        
        if (loreButton != null)
            loreButton.onClick.AddListener(() => OpenSection("Lore"));
        
        if (monstersButton != null)
            monstersButton.onClick.AddListener(() => OpenSection("Monsters"));
        
        if (spellsButton != null)
            spellsButton.onClick.AddListener(() => OpenSection("Spells"));
        
        if (itemsButton != null)
            itemsButton.onClick.AddListener(() => OpenSection("Items"));
        
        if (questsButton != null)
            questsButton.onClick.AddListener(() => OpenSection("Quests"));
    }
    
    private void InitializeTabStates()
    {
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
    }
    
    private void Update()
    {
        // Toggle book visibility with key press
        if (Input.GetKeyDown(bookToggleKey))
        {
            Debug.Log("Book toggle key pressed");
            ToggleBook();
        }
    }
    
    public void ToggleBook()
    {
        if (isBookOpen)
        {
            Debug.Log("Closing book");
            CloseBook();
        }
        else
        {
            Debug.Log("Opening book");
            OpenBook();
        }
    }
    
    public void OpenBook()
    {
        isBookOpen = true;
        if (bookCanvas != null)
        {
            bookCanvas.SetActive(true);
            Debug.Log("Book opened");
        }
        else
        {
            Debug.LogWarning("Book canvas reference is missing");
        }
        
        // Open the last section that was open
        OpenSection(lastOpenSection);
    }
    
    public void CloseBook()
    {
        isBookOpen = false;
        if (bookCanvas != null)
        {
            bookCanvas.SetActive(false);
            Debug.Log("Book closed");
        }
        else
        {
            Debug.LogWarning("Book canvas reference is missing");
        }
    }

    public void OpenSection(string sectionName)
    {
        // Save the last open section
        lastOpenSection = sectionName;
        
        // Hide all panels first
        HideAllSections();

        // Show the selected panel
        switch (sectionName)
        {
            case "Introduction":
                if (introductionPanel != null)
                    introductionPanel.SetActive(true);
                break;
            case "Lore":
                if (lorePanel != null)
                    lorePanel.SetActive(true);
                break;
            case "Monsters":
                if (monstersPanel != null)
                    monstersPanel.SetActive(true);
                break;
            case "Spells":
                if (spellsPanel != null)
                    spellsPanel.SetActive(true);
                break;
            case "Items":
                if (itemsPanel != null)
                    itemsPanel.SetActive(true);
                break;
            case "Quests":
                if (questsPanel != null)
                    questsPanel.SetActive(true);
                break;
        }
    }

    private void HideAllSections()
    {
        if (introductionPanel != null) introductionPanel.SetActive(false);
        if (lorePanel != null) lorePanel.SetActive(false);
        if (monstersPanel != null) monstersPanel.SetActive(false);
        if (spellsPanel != null) spellsPanel.SetActive(false);
        if (itemsPanel != null) itemsPanel.SetActive(false);
        if (questsPanel != null) questsPanel.SetActive(false);
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
        if (button == null) return;
        
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