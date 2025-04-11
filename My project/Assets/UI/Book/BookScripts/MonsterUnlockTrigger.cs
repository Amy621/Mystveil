using UnityEngine;

public class MonsterUnlockTrigger : MonoBehaviour
{
    [SerializeField] private string monsterName;
    private bool hasBeenUnlocked = false;
    private MonsterBookManager bookManager;

    private void Start()
    {
        // Find the book manager in the scene
        bookManager = FindObjectOfType<MonsterBookManager>();
        if (bookManager == null)
        {
            Debug.LogWarning($"MonsterBookManager not found in scene for monster: {monsterName}");
        }
    }

    // Call this when the monster dies
    public void OnMonsterDefeated()
    {
        if (hasBeenUnlocked || bookManager == null) return;
        
        bookManager.UnlockMonster(monsterName);
        hasBeenUnlocked = true;
    }

    // If your monster uses health system, you can hook this up to the health component
    public void OnHealthDepleted()
    {
        OnMonsterDefeated();
    }
} 