using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookTabManager : MonoBehaviour
{
    [Header("Page References")]
    [SerializeField] private GameObject introductionPage;
    [SerializeField] private GameObject lorePage;
    [SerializeField] private GameObject monstersPage;
    [SerializeField] private GameObject itemsPage;
    [SerializeField] private GameObject questsPage;
    [SerializeField] private GameObject spellsPage;

    [Header("Tab Button References")]
    [SerializeField] private Button introductionTab;
    [SerializeField] private Button loreTab;
    [SerializeField] private Button monstersTab;
    [SerializeField] private Button itemsTab;
    [SerializeField] private Button questsTab;
    [SerializeField] private Button spellsTab;

    [Header("Tab Visual Settings")]
    [SerializeField] private Color selectedTabColor = new Color(0.8f, 0.6f, 1f);
    [SerializeField] private Color unselectedTabColor = new Color(0.5f, 0.5f, 0.8f);

    private GameObject currentActivePage;
    private Button currentActiveTab;

    private void Start()
    {
        Debug.Log("BookTabManager Start - Setting up tabs");
        
        // Add listeners to all buttons
        if (introductionTab != null)
        {
            introductionTab.onClick.AddListener(OnIntroductionTabClicked);
            Debug.Log("Introduction tab listener added");
        }
        if (loreTab != null)
        {
            loreTab.onClick.AddListener(OnLoreTabClicked);
            Debug.Log("Lore tab listener added");
        }
        if (monstersTab != null)
        {
            monstersTab.onClick.AddListener(OnMonstersTabClicked);
            Debug.Log("Monsters tab listener added");
        }
        if (itemsTab != null)
        {
            itemsTab.onClick.AddListener(OnItemsTabClicked);
            Debug.Log("Items tab listener added");
        }
        if (questsTab != null)
        {
            questsTab.onClick.AddListener(OnQuestsTabClicked);
            Debug.Log("Quests tab listener added");
        }
        if (spellsTab != null)
        {
            spellsTab.onClick.AddListener(OnSpellsTabClicked);
            Debug.Log("Spells tab listener added");
        }

        // Start with Introduction page
        OnIntroductionTabClicked();
    }

    // Public methods that can be called from Unity's Inspector
    public void OnIntroductionTabClicked()
    {
        Debug.Log("Introduction tab clicked");
        SwitchToPage(introductionPage, introductionTab);
    }

    public void OnLoreTabClicked()
    {
        Debug.Log("Lore tab clicked");
        SwitchToPage(lorePage, loreTab);
    }

    public void OnMonstersTabClicked()
    {
        Debug.Log("Monsters tab clicked");
        SwitchToPage(monstersPage, monstersTab);
    }

    public void OnItemsTabClicked()
    {
        Debug.Log("Items tab clicked");
        SwitchToPage(itemsPage, itemsTab);
    }

    public void OnQuestsTabClicked()
    {
        Debug.Log("Quests tab clicked");
        SwitchToPage(questsPage, questsTab);
    }

    public void OnSpellsTabClicked()
    {
        Debug.Log("Spells tab clicked");
        SwitchToPage(spellsPage, spellsTab);
    }

    private void SwitchToPage(GameObject newPage, Button newTab)
    {
        if (newPage == null || newTab == null)
        {
            Debug.LogError("Attempting to switch to a null page or tab!");
            return;
        }

        Debug.Log($"Switching to page: {newPage.name}");

        // Deactivate current page and tab
        if (currentActivePage != null)
        {
            Debug.Log($"Deactivating current page: {currentActivePage.name}");
            currentActivePage.SetActive(false);
        }
        if (currentActiveTab != null)
        {
            Debug.Log($"Resetting current tab color: {currentActiveTab.name}");
            Image tabImage = currentActiveTab.GetComponent<Image>();
            if (tabImage != null)
            {
                tabImage.color = unselectedTabColor;
            }
        }

        // Activate new page and tab
        newPage.SetActive(true);
        Image newTabImage = newTab.GetComponent<Image>();
        if (newTabImage != null)
        {
            newTabImage.color = selectedTabColor;
        }

        // Update current references
        currentActivePage = newPage;
        currentActiveTab = newTab;
        Debug.Log($"Switch complete. Current page: {currentActivePage.name}");
    }

    private void OnValidate()
    {
        // This helps catch missing references in the Unity Editor
        if (introductionPage == null) Debug.LogWarning("Introduction Page not assigned!");
        if (lorePage == null) Debug.LogWarning("Lore Page not assigned!");
        if (monstersPage == null) Debug.LogWarning("Monsters Page not assigned!");
        if (itemsPage == null) Debug.LogWarning("Items Page not assigned!");
        if (questsPage == null) Debug.LogWarning("Quests Page not assigned!");
        if (spellsPage == null) Debug.LogWarning("Spells Page not assigned!");

        if (introductionTab == null) Debug.LogWarning("Introduction Tab not assigned!");
        if (loreTab == null) Debug.LogWarning("Lore Tab not assigned!");
        if (monstersTab == null) Debug.LogWarning("Monsters Tab not assigned!");
        if (itemsTab == null) Debug.LogWarning("Items Tab not assigned!");
        if (questsTab == null) Debug.LogWarning("Quests Tab not assigned!");
        if (spellsTab == null) Debug.LogWarning("Spells Tab not assigned!");
    }
} 