using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SpellPanelManager : MonoBehaviour 
{
    [Header("Spell UI References")]
    [SerializeField] private Transform spellListContent;
    [SerializeField] private Transform equippedSpellsContent;
    [SerializeField] private GameObject spellButtonPrefab;
    [SerializeField] private GameObject equippedSpellSlotPrefab;
    
    [Header("Spell Details Panel")]
    [SerializeField] private GameObject spellDetailsPanel;
    [SerializeField] private TextMeshProUGUI spellNameText;
    [SerializeField] private TextMeshProUGUI spellDescriptionText;
    [SerializeField] private TextMeshProUGUI spellCostText;
    [SerializeField] private TextMeshProUGUI spellCooldownText;
    [SerializeField] private Image spellIconImage;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;
    [SerializeField] private Button closeButton;
    
    // Property accessors for BookSystem
    public GameObject SpellButtonPrefab { get => spellButtonPrefab; set => spellButtonPrefab = value; }
    public GameObject EquippedSpellSlotPrefab { get => equippedSpellSlotPrefab; set => equippedSpellSlotPrefab = value; }
    public GameObject SpellDetailsPanel { get => spellDetailsPanel; set => spellDetailsPanel = value; }
    
    private SpellManager spellManager;
    private Spell selectedSpell;
    private List<GameObject> spellButtonObjects = new List<GameObject>();
    private List<GameObject> equippedSlotObjects = new List<GameObject>();
    
    private void OnEnable()
    {
        // Find the SpellManager
        if (spellManager == null)
        {
            spellManager = FindObjectOfType<SpellManager>();
            if (spellManager == null)
            {
                Debug.LogError("SpellPanelManager: Could not find SpellManager in the scene!");
                return;
            }
        }
        
        // Subscribe to spell changes
        spellManager.OnSpellsChanged += RefreshSpellDisplay;
        
        // Set up close button
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseSpellDetails);
        }
        
        // Update the UI
        RefreshSpellDisplay();
    }
    
    private void OnDisable()
    {
        // Unsubscribe from spell changes
        if (spellManager != null)
        {
            spellManager.OnSpellsChanged -= RefreshSpellDisplay;
        }
    }
    
    // Initialize references for the details panel - called by BookSystem
    public void InitializeDetailsPanelReferences()
    {
        if (spellDetailsPanel == null) return;
        
        // Find references if they're not already set
        if (spellNameText == null)
            spellNameText = spellDetailsPanel.transform.Find("Header/SpellName")?.GetComponent<TextMeshProUGUI>();
            
        if (spellDescriptionText == null)
            spellDescriptionText = spellDetailsPanel.transform.Find("Description/SpellDescription")?.GetComponent<TextMeshProUGUI>();
            
        if (spellCostText == null)
            spellCostText = spellDetailsPanel.transform.Find("SpellCost")?.GetComponent<TextMeshProUGUI>();
            
        if (spellCooldownText == null)
            spellCooldownText = spellDetailsPanel.transform.Find("SpellCooldown")?.GetComponent<TextMeshProUGUI>();
            
        if (spellIconImage == null)
            spellIconImage = spellDetailsPanel.transform.Find("SpellIcon")?.GetComponent<Image>();
            
        if (equipButton == null)
            equipButton = spellDetailsPanel.transform.Find("ButtonContainer/EquipButton")?.GetComponent<Button>();
            
        if (unequipButton == null)
            unequipButton = spellDetailsPanel.transform.Find("ButtonContainer/UnequipButton")?.GetComponent<Button>();
            
        if (closeButton == null)
            closeButton = spellDetailsPanel.transform.Find("CloseButton")?.GetComponent<Button>();
            
        // Set up close button
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseSpellDetails);
        }
        
        // Ensure the panel starts inactive
        spellDetailsPanel.SetActive(false);
    }
    
    public void RefreshSpellDisplay()
    {
        // Don't refresh if we don't have the prefabs
        if (spellButtonPrefab == null || equippedSpellSlotPrefab == null)
        {
            Debug.LogWarning("SpellPanelManager: Button prefabs not assigned. Cannot refresh display.");
            return;
        }
        
        // Check if we have the content transforms
        if (spellListContent == null || equippedSpellsContent == null)
        {
            Debug.LogWarning("SpellPanelManager: Content transforms not assigned. Cannot refresh display.");
            return;
        }
        
        // Clear existing spell buttons
        foreach (GameObject buttonObj in spellButtonObjects)
        {
            Destroy(buttonObj);
        }
        spellButtonObjects.Clear();
        
        // Clear existing equipped spell slots
        foreach (GameObject slotObj in equippedSlotObjects)
        {
            Destroy(slotObj);
        }
        equippedSlotObjects.Clear();
        
        // Make sure we have a SpellManager
        if (spellManager == null)
        {
            spellManager = FindObjectOfType<SpellManager>();
            if (spellManager == null)
            {
                Debug.LogError("SpellPanelManager: Could not find SpellManager in the scene!");
                return;
            }
        }
        
        // Get all unlocked spells
        List<Spell> unlockedSpells = spellManager.GetUnlockedSpellsList();
        
        // Create buttons for each unlocked spell
        foreach (Spell spell in unlockedSpells)
        {
            GameObject buttonObj = Instantiate(spellButtonPrefab, spellListContent);
            spellButtonObjects.Add(buttonObj);
            
            // Set up the button
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI nameText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            
            if (nameText != null)
            {
                nameText.text = spell.SpellName;
            }
            
            // Add listener to show spell details
            string spellID = spell.SpellID; // Capture for closure
            button.onClick.AddListener(() => ShowSpellDetails(spellID));
            
            // Visual indicator if equipped
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null && spellManager.IsSpellEquipped(spell.SpellID))
            {
                buttonImage.color = new Color(0.7f, 0.9f, 0.7f);
            }
        }
        
        // Create slots for equipped spells
        int maxSlots = spellManager.GetMaxEquippedSpells();
        List<Spell> equippedSpells = spellManager.GetEquippedSpellsList();
        
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObj = Instantiate(equippedSpellSlotPrefab, equippedSpellsContent);
            equippedSlotObjects.Add(slotObj);
            
            // Set up the slot
            int slotIndex = i; // Capture for closure
            Button slotButton = slotObj.GetComponent<Button>();
            TextMeshProUGUI slotText = slotObj.GetComponentInChildren<TextMeshProUGUI>();
            
            // Label the slot
            TextMeshProUGUI slotLabel = slotObj.transform.Find("SlotLabel")?.GetComponent<TextMeshProUGUI>();
            if (slotLabel != null)
            {
                slotLabel.text = $"Slot {i + 1}";
            }
            
            // Get the spell in this slot (if any)
            Spell slotSpell = i < equippedSpells.Count ? equippedSpells[i] : null;
            
            if (slotText != null)
            {
                slotText.text = slotSpell != null ? slotSpell.SpellName : "Empty";
            }
            
            // Add listener to select this slot for equipping
            slotButton.onClick.AddListener(() => OnSlotClicked(slotIndex));
        }
        
        // Hide details panel by default
        if (spellDetailsPanel != null)
        {
            spellDetailsPanel.SetActive(false);
        }
    }
    
    private void ShowSpellDetails(string spellID)
    {
        // Check if we have all the required components
        if (spellDetailsPanel == null || spellNameText == null || spellDescriptionText == null || 
            spellCostText == null || equipButton == null || unequipButton == null)
        {
            Debug.LogError("SpellPanelManager: Missing required components for spell details panel.");
            return;
        }
        
        selectedSpell = spellManager.GetSpell(spellID);
        if (selectedSpell == null) return;
        
        // Show the details panel
        spellDetailsPanel.SetActive(true);
        
        // Update UI elements
        spellNameText.text = selectedSpell.SpellName;
        spellDescriptionText.text = selectedSpell.Description;
        spellCostText.text = $"Cost: {selectedSpell.ManaCost} MP";
        
        if (spellCooldownText != null)
            spellCooldownText.text = $"Cooldown: {selectedSpell.Cooldown}s";
        
        // Handle icon if it exists
        if (spellIconImage != null)
        {
            // Since the Spell class doesn't have an Icon property,
            // we'll just hide the icon element
            spellIconImage.gameObject.SetActive(false);
        }
        
        // Set up buttons
        bool isEquipped = spellManager.IsSpellEquipped(spellID);
        equipButton.gameObject.SetActive(!isEquipped);
        unequipButton.gameObject.SetActive(isEquipped);
        
        equipButton.onClick.RemoveAllListeners();
        unequipButton.onClick.RemoveAllListeners();
        
        equipButton.onClick.AddListener(() => EquipSelectedSpell());
        unequipButton.onClick.AddListener(() => UnequipSelectedSpell());
    }
    
    private void CloseSpellDetails()
    {
        if (spellDetailsPanel != null)
        {
            spellDetailsPanel.SetActive(false);
        }
        selectedSpell = null;
    }
    
    private void EquipSelectedSpell()
    {
        if (selectedSpell == null) return;
        
        spellManager.EquipSpell(selectedSpell.SpellID);
        RefreshSpellDisplay();
    }
    
    private void UnequipSelectedSpell()
    {
        if (selectedSpell == null) return;
        
        spellManager.UnequipSpell(selectedSpell.SpellID);
        RefreshSpellDisplay();
    }
    
    private void OnSlotClicked(int slotIndex)
    {
        if (selectedSpell == null) return;
        
        spellManager.EquipSpellToSlot(selectedSpell.SpellID, slotIndex);
        RefreshSpellDisplay();
    }
} 