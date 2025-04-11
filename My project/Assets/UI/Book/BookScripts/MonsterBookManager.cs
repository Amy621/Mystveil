using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MonsterBookManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform monsterGridContainer;
    [SerializeField] private GameObject monsterCardPrefab;
    [SerializeField] private GameObject monsterDetailPanel;
    
    [Header("Detail View References")]
    [SerializeField] private Image detailImage;
    [SerializeField] private TextMeshProUGUI monsterNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statsText;
    
    [Header("Monster Data")]
    [SerializeField] private List<MonsterData> allMonsters = new List<MonsterData>();
    
    private Dictionary<string, MonsterData> monsterDictionary = new Dictionary<string, MonsterData>();
    
    private void Awake()
    {
        Debug.Log("MonsterBookManager Awake - Initializing");
        // Initialize dictionary
        foreach (var monster in allMonsters)
        {
            if (monster != null)
            {
                monsterDictionary[monster.monsterName] = monster;
                Debug.Log($"Added monster to dictionary: {monster.monsterName}");
            }
        }
        
        // Initially populate grid with undiscovered monsters
        PopulateMonsterGrid();
        
        // Ensure detail panel is set up
        if (monsterDetailPanel != null)
        {
            monsterDetailPanel.SetActive(true); // Make sure it's initially visible
        }
        else
        {
            Debug.LogError("Monster Detail Panel is not assigned!");
        }
    }
    
    private void PopulateMonsterGrid()
    {
        Debug.Log("Populating monster grid");
        // Clear existing cards
        foreach (Transform child in monsterGridContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create cards for each monster
        foreach (var monster in allMonsters)
        {
            if (monster != null)
            {
                GameObject card = Instantiate(monsterCardPrefab, monsterGridContainer);
                SetupMonsterCard(card, monster);
                Debug.Log($"Created card for monster: {monster.monsterName}");
            }
        }
    }
    
    private void SetupMonsterCard(GameObject card, MonsterData monster)
    {
        // Get components
        Image iconImage = card.GetComponentInChildren<Image>();
        TextMeshProUGUI nameText = card.GetComponentInChildren<TextMeshProUGUI>();
        Button cardButton = card.GetComponent<Button>();
        
        if (iconImage == null) Debug.LogError($"Icon Image component missing on card for {monster.monsterName}");
        if (nameText == null) Debug.LogError($"TextMeshPro component missing on card for {monster.monsterName}");
        if (cardButton == null) Debug.LogError($"Button component missing on card for {monster.monsterName}");
        
        // Set data
        if (monster.isDiscovered)
        {
            Debug.Log($"Setting up discovered monster card: {monster.monsterName}");
            if (monster.monsterIcon != null)
            {
                iconImage.sprite = monster.monsterIcon;
            }
            else
            {
                Debug.LogError($"Monster icon is missing for {monster.monsterName}");
            }
            nameText.text = monster.monsterName;
            cardButton.onClick.AddListener(() => ShowMonsterDetail(monster));
        }
        else
        {
            Debug.Log($"Setting up undiscovered monster card: {monster.monsterName}");
            iconImage.color = Color.black; // Silhouette
            nameText.text = "???";
            cardButton.interactable = false;
        }
    }
    
    private void ShowMonsterDetail(MonsterData monster)
    {
        Debug.Log($"Showing detail for monster: {monster.monsterName}");
        
        if (monsterDetailPanel == null)
        {
            Debug.LogError("Detail panel is null!");
            return;
        }
        
        monsterDetailPanel.SetActive(true);
        
        if (detailImage == null || monsterNameText == null || descriptionText == null || statsText == null)
        {
            Debug.LogError("One or more detail view components are not assigned!");
            return;
        }
        
        if (monster.monsterDetailImage == null)
        {
            Debug.LogError($"Detail image is missing for {monster.monsterName}");
        }
        else
        {
            detailImage.sprite = monster.monsterDetailImage;
        }
        
        monsterNameText.text = monster.monsterName;
        descriptionText.text = monster.description;
        
        statsText.text = $"Health: {monster.health}\n" +
                        $"Damage: {monster.damage}\n" +
                        $"Habitat: {monster.habitat}\n" +
                        $"Behavior: {monster.behavior}\n" +
                        $"Weaknesses: {monster.weaknesses}";
                        
        Debug.Log("Monster detail view updated successfully");
    }
    
    // Call this method when a monster is defeated for the first time
    public void UnlockMonster(string monsterName)
    {
        Debug.Log($"Attempting to unlock monster: {monsterName}");
        if (monsterDictionary.TryGetValue(monsterName, out MonsterData monster))
        {
            monster.isDiscovered = true;
            Debug.Log($"Successfully unlocked monster: {monsterName}");
            PopulateMonsterGrid(); // Refresh the grid
        }
        else
        {
            Debug.LogError($"Failed to find monster with name: {monsterName}");
        }
    }
} 