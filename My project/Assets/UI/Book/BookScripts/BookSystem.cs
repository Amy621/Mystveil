using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BookSystem : MonoBehaviour
{
    public static BookSystem Instance { get; private set; }
    
    [Header("Prefab References")]
    [SerializeField] private GameObject bookCanvasPrefab;
    [SerializeField] private GameObject spellButtonPrefab;
    [SerializeField] private GameObject equippedSpellSlotPrefab;
    [SerializeField] private GameObject spellDetailsPanelPrefab;
    
    [Header("Settings")]
    [SerializeField] private KeyCode bookToggleKey = KeyCode.B;
    [SerializeField] private bool startWithAllTabsLocked = true;
    [SerializeField] private bool introductionUnlockedByDefault = true;
    
    private BookManager bookManager;
    private GameObject bookCanvasInstance;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            InitializeBookSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeBookSystem()
    {
        Debug.Log("Initializing Book System...");
        
        // Instantiate the book canvas prefab if it's provided
        if (bookCanvasPrefab != null)
        {
            // Destroy any existing instance first
            if (bookCanvasInstance != null)
            {
                Destroy(bookCanvasInstance);
            }
            
            bookCanvasInstance = Instantiate(bookCanvasPrefab);
            
            // Make it a child of this GameObject
            bookCanvasInstance.transform.SetParent(transform);
            
            // Reset transform to ensure proper positioning
            bookCanvasInstance.transform.localPosition = Vector3.zero;
            bookCanvasInstance.transform.localRotation = Quaternion.identity;
            bookCanvasInstance.transform.localScale = Vector3.one;
            
            bookManager = bookCanvasInstance.GetComponent<BookManager>();
            
            if (bookManager == null)
            {
                Debug.LogError("Book Canvas prefab does not contain a BookManager component!");
                return;
            }
            
            // Configure book manager settings
            bookManager.BookToggleKey = bookToggleKey;
            bookManager.StartWithAllTabsLocked = startWithAllTabsLocked;
            bookManager.IntroductionUnlockedByDefault = introductionUnlockedByDefault;
            
            Debug.Log("BookSystem: Key to toggle book is set to: " + bookToggleKey);
            Debug.Log("BookSystem: Book canvas instance name: " + bookCanvasInstance.name);
            
            // Make sure it starts inactive
            bookCanvasInstance.SetActive(false);
            
            // Setup spell panel manager if it exists
            SetupSpellPanelManager();
        }
        else
        {
            Debug.LogError("Book Canvas prefab is not assigned! The Book System won't function properly.");
        }
    }
    
    private void SetupSpellPanelManager()
    {
        if (bookManager == null)
        {
            Debug.LogError("BookManager is null, cannot set up SpellPanelManager");
            return;
        }
        
        if (bookManager.SpellsPanel == null)
        {
            Debug.LogWarning("Spells panel not found in the Book Canvas prefab.");
            return;
        }
        
        SpellPanelManager spellPanel = bookManager.SpellsPanel.GetComponent<SpellPanelManager>();
        if (spellPanel == null)
        {
            Debug.LogWarning("SpellPanelManager component not found on the Spells panel.");
            return;
        }
        
        // Assign prefabs to the spell panel manager
        if (spellButtonPrefab != null)
        {
            spellPanel.SpellButtonPrefab = spellButtonPrefab;
            Debug.Log("Assigned SpellButtonPrefab to SpellPanelManager");
        }
        
        if (equippedSpellSlotPrefab != null)
        {
            spellPanel.EquippedSpellSlotPrefab = equippedSpellSlotPrefab;
            Debug.Log("Assigned EquippedSpellSlotPrefab to SpellPanelManager");
        }
        
        // Set up details panel if not already present
        if (spellPanel.SpellDetailsPanel == null && spellDetailsPanelPrefab != null)
        {
            GameObject detailsPanel = Instantiate(spellDetailsPanelPrefab, spellPanel.transform);
            spellPanel.SpellDetailsPanel = detailsPanel;
            Debug.Log("Created SpellDetailsPanel for SpellPanelManager");
            
            // Initialize reference connections for the details panel
            spellPanel.InitializeDetailsPanelReferences();
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Make sure the book is closed when changing scenes
        if (bookManager != null)
        {
            bookManager.CloseBook();
        }
    }
    
    // Public methods to interact with the book system
    
    public void ToggleBook()
    {
        if (bookManager != null)
        {
            bookManager.ToggleBook();
        }
        else
        {
            Debug.LogError("BookSystem: Cannot toggle book - BookManager is null");
            // Try to re-initialize if the book manager is missing
            InitializeBookSystem();
        }
    }
    
    public void OpenBook()
    {
        if (bookManager != null)
        {
            bookManager.OpenBook();
        }
        else
        {
            Debug.LogError("BookSystem: Cannot open book - BookManager is null");
            // Try to re-initialize if the book manager is missing
            InitializeBookSystem();
        }
    }
    
    public void CloseBook()
    {
        if (bookManager != null)
        {
            bookManager.CloseBook();
        }
        else
        {
            Debug.LogError("BookSystem: Cannot close book - BookManager is null");
        }
    }
    
    public void UnlockTab(string tabName)
    {
        if (bookManager != null)
        {
            bookManager.UnlockTab(tabName);
        }
        else
        {
            Debug.LogError("BookSystem: Cannot unlock tab - BookManager is null");
        }
    }
    
    // Allow toggling the book directly with the B key
    private void Update()
    {
        if (Input.GetKeyDown(bookToggleKey))
        {
            Debug.Log("BookSystem: Toggle key pressed directly from BookSystem");
            ToggleBook();
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
} 