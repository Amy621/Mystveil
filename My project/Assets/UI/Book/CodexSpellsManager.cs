using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

/// <summary>
/// Manages the Spells page in the Enchanted Codex.
/// Displays known spells, allows equipping/unequipping, and shows spell details.
/// </summary>
public class CodexSpellsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform spellsContent;
    [SerializeField] private GameObject spellEntryPrefab;
    
    [Header("Spell Details Panel")]
    [SerializeField] private GameObject spellDetailsPanel;
    [SerializeField] private TextMeshProUGUI spellNameText;
    [SerializeField] private TextMeshProUGUI spellTypeText;
    [SerializeField] private TextMeshProUGUI spellDescriptionText;
    [SerializeField] private TextMeshProUGUI spellStatsText;
    [SerializeField] private TextMeshProUGUI spellManaText;
    [SerializeField] private Image spellIconImage;
    [SerializeField] private Button closeDetailsButton;
    
    [Header("Settings")]
    [SerializeField] private Color commonSpellColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color rareSpellColor = new Color(0.4f, 0.6f, 1f);
    [SerializeField] private Color epicSpellColor = new Color(0.8f, 0.4f, 1f);

    private Dictionary<string, ScriptableSpell> discoveredSpells = new Dictionary<string, ScriptableSpell>();
    private List<GameObject> spellEntryObjects = new List<GameObject>();
    private string selectedSpellId;
    private bool initialized = false;

    public void Initialize()
    {
        if (initialized) return;
        
        // Set up close button
        if (closeDetailsButton != null)
        {
            closeDetailsButton.onClick.RemoveAllListeners();
            closeDetailsButton.onClick.AddListener(CloseSpellDetails);
        }
        
        // Hide details panel initially
        if (spellDetailsPanel != null)
        {
            spellDetailsPanel.SetActive(false);
        }
        
        // Load spells
        LoadSpells();
        
        initialized = true;
        Debug.Log("CodexSpellsManager initialized");
    }
    
    private void OnEnable()
    {
        // Refresh content when shown
        RefreshContent();
    }
    
    private void OnDisable()
    {
        // Close details panel if showing
        if (spellDetailsPanel != null && spellDetailsPanel.activeSelf)
        {
            spellDetailsPanel.SetActive(false);
        }
    }
    
    private void LoadSpells()
    {
        // Load all spells from Resources folder
        ScriptableSpell[] allSpells = Resources.LoadAll<ScriptableSpell>("1V1/PlayerSpells");
        
        if (allSpells != null && allSpells.Length > 0)
        {
            foreach (var spell in allSpells)
            {
                // Add to discovered spells (in a real game, you'd check if discovered)
                discoveredSpells[spell.name] = spell;
            }
            
            Debug.Log($"Loaded {discoveredSpells.Count} spells");
        }
        else
        {
            Debug.LogWarning("No spells found in Resources/1V1/PlayerSpells");
        }
    }
    
    public void RefreshContent()
    {
        // Clear existing spell entries
        foreach (GameObject entryObj in spellEntryObjects)
        {
            Destroy(entryObj);
        }
        spellEntryObjects.Clear();
        
        // Sort spells by type and name
        List<ScriptableSpell> sortedSpells = new List<ScriptableSpell>(discoveredSpells.Values);
        sortedSpells.Sort((a, b) => {
            // First by type
            int typeComparison = a.spellType.CompareTo(b.spellType);
            if (typeComparison != 0) return typeComparison;
            
            // Then by name
            return a.name.CompareTo(b.name);
        });
        
        // Create an entry for each spell
        foreach (ScriptableSpell spell in sortedSpells)
        {
            GameObject entryObj = Instantiate(spellEntryPrefab, spellsContent);
            spellEntryObjects.Add(entryObj);
            
            // Set up the entry
            Button button = entryObj.GetComponent<Button>();
            TextMeshProUGUI nameText = entryObj.GetComponentInChildren<TextMeshProUGUI>();
            Image iconImage = entryObj.transform.Find("SpellIcon")?.GetComponent<Image>();
            
            if (nameText != null)
            {
                nameText.text = spell.name;
            }
            
            if (iconImage != null && spell.icon != null)
            {
                iconImage.sprite = spell.icon;
            }
            
            // Set color based on spell rarity/power
            Image entryImage = entryObj.GetComponent<Image>();
            if (entryImage != null)
            {
                // Example logic - adjust based on your spell system
                if (spell.manaCost >= 50)
                    entryImage.color = epicSpellColor;
                else if (spell.manaCost >= 30)
                    entryImage.color = rareSpellColor;
                else
                    entryImage.color = commonSpellColor;
            }
            
            // Add listener to show spell details
            button.onClick.AddListener(() => ShowSpellDetails(spell));
        }
    }
    
    private void ShowSpellDetails(ScriptableSpell spell)
    {
        selectedSpellId = spell.name;
        
        // Make sure we have necessary components
        if (spellDetailsPanel == null || spellNameText == null)
            return;
            
        // Show the details panel
        spellDetailsPanel.SetActive(true);
        
        // Update UI elements
        spellNameText.text = spell.name;
        
        if (spellTypeText != null)
            spellTypeText.text = spell.spellType.ToString();
            
        if (spellDescriptionText != null)
            spellDescriptionText.text = spell.description;
            
        if (spellManaText != null)
            spellManaText.text = $"Mana Cost: {spell.manaCost}";
            
        if (spellStatsText != null)
        {
            string statsStr = "Stats:\n";
            statsStr += $"• Damage: {spell.damage}\n";
            statsStr += $"• Cooldown: {spell.cooldown}s\n";
            
            // Add more spell stats here
            
            spellStatsText.text = statsStr;
        }
        
        if (spellIconImage != null && spell.icon != null)
            spellIconImage.sprite = spell.icon;
    }
    
    private void CloseSpellDetails()
    {
        if (spellDetailsPanel != null)
        {
            spellDetailsPanel.SetActive(false);
        }
        selectedSpellId = null;
    }
    
    public void OnSave(SimpleSaveData saveData)
    {
        // Save discovered spell IDs
        saveData.SetStringArray("discovered_spells", discoveredSpells.Keys.ToArray());
    }
    
    public void OnLoad(SimpleSaveData saveData)
    {
        // Clear current spells
        discoveredSpells.Clear();
        
        // Get all possible spells
        Dictionary<string, ScriptableSpell> allSpells = new Dictionary<string, ScriptableSpell>();
        foreach (var spell in Resources.LoadAll<ScriptableSpell>("1V1/PlayerSpells"))
        {
            allSpells[spell.name] = spell;
        }
        
        // Load discovered spells
        string[] discoveredIds = saveData.GetStringArray("discovered_spells");
        if (discoveredIds != null && discoveredIds.Length > 0)
        {
            foreach (string id in discoveredIds)
            {
                if (allSpells.ContainsKey(id))
                {
                    discoveredSpells[id] = allSpells[id];
                }
            }
        }
        else
        {
            // For testing, just add all spells as discovered
            discoveredSpells = new Dictionary<string, ScriptableSpell>(allSpells);
        }
        
        // Refresh UI
        RefreshContent();
    }
} 