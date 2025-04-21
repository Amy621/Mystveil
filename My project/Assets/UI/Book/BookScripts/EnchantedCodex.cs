using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Main controller for the Enchanted Codex book system.
/// This is a singleton that persists across scenes and handles all book functionality.
/// </summary>
public class EnchantedCodex : MonoBehaviour
{
    public static EnchantedCodex Instance { get; private set; }

    [Header("Book UI")]
    [SerializeField] private Canvas bookCanvas;
    [SerializeField] private GameObject bookBackground;
    [SerializeField] private GameObject pagesContainer;
    [SerializeField] private GameObject tabsContainer;
    [SerializeField] private KeyCode toggleKey = KeyCode.B;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image pageLeftImage;
    [SerializeField] private Image pageRightImage;

    [Header("Tab References")]
    [SerializeField] private Button introTab;
    [SerializeField] private Button questsTab;
    [SerializeField] private Button monstersTab;
    [SerializeField] private Button spellsTab;
    [SerializeField] private Button itemsTab;
    [SerializeField] private Button loreTab;

    [Header("Page References")]
    [SerializeField] private GameObject introPage;
    [SerializeField] private GameObject questsPage;
    [SerializeField] private GameObject monstersPage;
    [SerializeField] private GameObject spellsPage;
    [SerializeField] private GameObject itemsPage;
    [SerializeField] private GameObject lorePage;

    [Header("Page Managers")]
    [SerializeField] private CodexSpellsManager spellsManager;
    [SerializeField] private CodexQuestsManager questsManager;
    [SerializeField] private CodexMonstersManager monstersManager;

    [Header("Animation")]
    [SerializeField] private float openAnimationTime = 0.5f;
    [SerializeField] private AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // State tracking
    private bool isOpen = false;
    private string currentPage = "Intro";
    private Dictionary<string, bool> unlockedTabs = new Dictionary<string, bool>();
    private bool initialized = false;

    // SaveSystem integration
    private SimpleSaveSystem saveSystem;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            
            // Make both this object AND the parent Canvas persistent
            Canvas parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                DontDestroyOnLoad(parentCanvas.gameObject);
                Debug.Log("Made parent Canvas persistent across scenes");
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                Debug.LogWarning("No parent Canvas found, making only EnchantedCodex persistent");
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize default unlocked tabs
        unlockedTabs["Intro"] = true;
        unlockedTabs["Quests"] = true;
        unlockedTabs["Monsters"] = false;
        unlockedTabs["Spells"] = false;
        unlockedTabs["Items"] = false;
        unlockedTabs["Lore"] = false;

        // Initialize the book
        InitializeBook();
    }

    private void Start()
    {
        // Find the save system
        saveSystem = FindObjectOfType<SimpleSaveSystem>();
        if (saveSystem != null)
        {
            // Hook into save/load events
            saveSystem.OnSave += OnSaveGame;
            saveSystem.OnLoad += OnLoadGame;
            Debug.Log("EnchantedCodex connected to SaveSystem");
        }
        else
        {
            Debug.LogWarning("SimpleSaveSystem not found - save functionality will be limited");
        }

        // Close the book at start
        CloseBook(false);
    }

    private void InitializeBook()
    {
        if (initialized) return;

        // Set up close button
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => CloseBook());
        }

        // Set up tab buttons
        SetupTabButton(introTab, "Intro");
        SetupTabButton(questsTab, "Quests");
        SetupTabButton(monstersTab, "Monsters");
        SetupTabButton(spellsTab, "Spells");
        SetupTabButton(itemsTab, "Items");
        SetupTabButton(loreTab, "Lore");

        // Initialize page managers if they exist
        if (spellsManager != null)
        {
            spellsManager.Initialize();
        }
        
        if (questsManager != null)
        {
            questsManager.Initialize();
        }
        
        if (monstersManager != null)
        {
            monstersManager.Initialize();
        }

        initialized = true;
        Debug.Log("EnchantedCodex initialized");
    }

    private void SetupTabButton(Button button, string pageName)
    {
        if (button == null) return;

        // Remove existing listeners
        button.onClick.RemoveAllListeners();
        
        // Add new listener that ONLY opens the page
        // Important: this should not toggle the book state
        button.onClick.AddListener(() => {
            // Only open the page if the book is already open
            if (isOpen) 
            {
                OpenPage(pageName);
                Debug.Log($"Tab button clicked for {pageName}");
            }
        });
        
        // Set initial state based on unlocked status
        UpdateTabVisuals(button, pageName);
    }

    private void UpdateTabVisuals(Button tab, string pageName)
    {
        if (tab == null) return;

        bool isUnlocked = unlockedTabs.ContainsKey(pageName) && unlockedTabs[pageName];
        bool isActive = currentPage == pageName && isOpen;

        // Update interactability
        tab.interactable = isUnlocked;

        // Update visual state
        ColorBlock colors = tab.colors;
        if (!isUnlocked)
        {
            // Locked state
            colors.normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        else if (isActive)
        {
            // Active state
            colors.normalColor = new Color(1f, 0.8f, 0.4f, 1f);
        }
        else
        {
            // Unlocked but inactive
            colors.normalColor = Color.white;
        }
        tab.colors = colors;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Close the book when changing scenes but don't destroy canvas
        CloseBook(false);
    }

    private void Update()
    {
        // Toggle book with key press
        if (Input.GetKeyDown(toggleKey))
        {
            Debug.Log($"Toggle key pressed. Current state: isOpen={isOpen}, currentPage={currentPage}");
            ToggleBook();
        }
    }

    /// <summary>
    /// Toggles the book open/closed.
    /// </summary>
    public void ToggleBook()
    {
        Debug.Log($"ToggleBook called. Before toggle: isOpen={isOpen}");
        
        if (isOpen)
        {
            CloseBook();
        }
        else
        {
            OpenBook();
        }
        
        Debug.Log($"After toggle: isOpen={isOpen}");
    }

    /// <summary>
    /// Opens the book to the last viewed page.
    /// </summary>
    public void OpenBook()
    {
        if (isOpen)
        {
            Debug.Log("Book is already open, ignoring OpenBook call");
            return;
        }

        // Enable the canvas and containers
        if (bookCanvas != null) 
        {
            bookCanvas.gameObject.SetActive(true);
        }
        
        if (bookBackground != null)
        {
            bookBackground.SetActive(true);
        }
        
        if (pagesContainer != null)
        {
            pagesContainer.SetActive(true);
        }
        
        if (tabsContainer != null)
        {
            tabsContainer.SetActive(true);
        }

        // Open the current page
        OpenPage(currentPage);
        
        // Set state flag - do this AFTER opening page
        isOpen = true;

        Debug.Log($"EnchantedCodex opened to page {currentPage}. isOpen={isOpen}");
    }

    /// <summary>
    /// Closes the book.
    /// </summary>
    /// <param name="animate">Whether to animate the closing</param>
    public void CloseBook(bool animate = true)
    {
        if (!isOpen)
        {
            Debug.Log("Book is already closed, ignoring CloseBook call");
            return;
        }

        // Set state flag first to prevent page reopening
        isOpen = false;

        // Hide all UI elements
        if (bookBackground != null)
        {
            bookBackground.SetActive(false);
        }
        
        if (pagesContainer != null)
        {
            pagesContainer.SetActive(false);
        }
        
        if (tabsContainer != null)
        {
            tabsContainer.SetActive(false);
        }
        
        // Keep canvas active but hide content
        if (bookCanvas != null)
        {
            // Don't deactivate the canvas itself, just ensure content is hidden
            // bookCanvas.gameObject.SetActive(false);
        }

        Debug.Log($"EnchantedCodex closed. isOpen={isOpen}");
    }

    /// <summary>
    /// Opens a specific page in the book.
    /// </summary>
    /// <param name="pageName">The name of the page to open</param>
    public void OpenPage(string pageName)
    {
        if (!isOpen)
        {
            // Store the page we want to open when the book is opened
            currentPage = pageName;
            Debug.Log($"Book is closed, storing {pageName} as page to open");
            return;
        }

        // Check if the page is unlocked
        if (!unlockedTabs.ContainsKey(pageName) || !unlockedTabs[pageName])
        {
            Debug.LogWarning($"Cannot open page {pageName} - it is locked");
            return;
        }

        // Hide all pages
        HideAllPages();

        // Show the requested page
        currentPage = pageName;
        GameObject pageToShow = GetPageByName(pageName);
        if (pageToShow != null)
        {
            pageToShow.SetActive(true);
        }

        // Update tab visuals
        UpdateAllTabVisuals();

        // Refresh page content
        RefreshPageContent(pageName);

        Debug.Log($"Opened book page: {pageName}");
    }

    /// <summary>
    /// Unlocks a tab in the book, making it accessible to the player.
    /// </summary>
    /// <param name="tabName">The name of the tab to unlock</param>
    public void UnlockTab(string tabName)
    {
        if (unlockedTabs.ContainsKey(tabName))
        {
            unlockedTabs[tabName] = true;
            UpdateAllTabVisuals();
            Debug.Log($"Unlocked book tab: {tabName}");
        }
    }

    private void HideAllPages()
    {
        if (introPage != null) introPage.SetActive(false);
        if (questsPage != null) questsPage.SetActive(false);
        if (monstersPage != null) monstersPage.SetActive(false);
        if (spellsPage != null) spellsPage.SetActive(false);
        if (itemsPage != null) itemsPage.SetActive(false);
        if (lorePage != null) lorePage.SetActive(false);
    }

    private GameObject GetPageByName(string pageName)
    {
        switch (pageName)
        {
            case "Intro": return introPage;
            case "Quests": return questsPage;
            case "Monsters": return monstersPage;
            case "Spells": return spellsPage;
            case "Items": return itemsPage;
            case "Lore": return lorePage;
            default: return null;
        }
    }

    private void UpdateAllTabVisuals()
    {
        UpdateTabVisuals(introTab, "Intro");
        UpdateTabVisuals(questsTab, "Quests");
        UpdateTabVisuals(monstersTab, "Monsters");
        UpdateTabVisuals(spellsTab, "Spells");
        UpdateTabVisuals(itemsTab, "Items");
        UpdateTabVisuals(loreTab, "Lore");
    }

    private void RefreshPageContent(string pageName)
    {
        switch (pageName)
        {
            case "Spells":
                if (spellsManager != null) spellsManager.RefreshContent();
                break;
            case "Quests":
                if (questsManager != null) questsManager.RefreshContent();
                break;
            case "Monsters":
                if (monstersManager != null) monstersManager.RefreshContent();
                break;
        }
    }

    // SaveSystem integration

    private void OnSaveGame(SimpleSaveData saveData)
    {
        // Save unlocked tabs
        saveData.unlockedBookTabs = new List<string>();
        foreach (var tab in unlockedTabs)
        {
            if (tab.Value)
            {
                saveData.unlockedBookTabs.Add(tab.Key);
            }
        }

        // Let individual managers save their data
        if (spellsManager != null) spellsManager.OnSave(saveData);
        if (questsManager != null) questsManager.OnSave(saveData);
        if (monstersManager != null) monstersManager.OnSave(saveData);

        Debug.Log("EnchantedCodex saved data");
    }

    private void OnLoadGame(SimpleSaveData saveData)
    {
        // Load unlocked tabs
        if (saveData.unlockedBookTabs != null)
        {
            // Reset all tabs to locked
            foreach (var key in unlockedTabs.Keys)
            {
                unlockedTabs[key] = false;
            }
            
            // Always ensure Intro is unlocked
            unlockedTabs["Intro"] = true;

            // Unlock saved tabs
            foreach (var tab in saveData.unlockedBookTabs)
            {
                if (unlockedTabs.ContainsKey(tab))
                {
                    unlockedTabs[tab] = true;
                }
            }

            UpdateAllTabVisuals();
        }

        // Let individual managers load their data
        if (spellsManager != null) spellsManager.OnLoad(saveData);
        if (questsManager != null) questsManager.OnLoad(saveData);
        if (monstersManager != null) monstersManager.OnLoad(saveData);

        Debug.Log("EnchantedCodex loaded data");
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        if (saveSystem != null)
        {
            saveSystem.OnSave -= OnSaveGame;
            saveSystem.OnLoad -= OnLoadGame;
        }
    }
} 