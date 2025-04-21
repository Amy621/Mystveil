using UnityEngine;

/// <summary>
/// Component to be attached to monster prefabs to enable discovery in the monster book.
/// </summary>
public class MonsterDiscoveryComponent : MonoBehaviour
{
    [Tooltip("Name of the EnemyStats scriptable object this monster relates to")]
    [SerializeField] private string monsterName;
    
    [Tooltip("If true, monster will be discovered when this GameObject is destroyed")]
    [SerializeField] private bool discoverOnDeath = true;
    
    [Tooltip("If true, monster will be discovered when player gets within range")]
    [SerializeField] private bool discoverOnSight = false;
    
    [Tooltip("Range at which the monster is considered 'seen' if discoverOnSight is true")]
    [SerializeField] private float sightRange = 10f;
    
    private bool discovered = false;
    private Transform playerTransform;
    private MonsterBookManager bookManager;
    
    private void Start()
    {
        // Try to find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        // Find the monster book manager
        bookManager = FindObjectOfType<MonsterBookManager>();
        if (bookManager == null)
        {
            Debug.LogWarning("MonsterBookManager not found in the scene. Monster discovery will not work.");
        }
        
        // If no monster name is specified, try to guess from this GameObject's name
        if (string.IsNullOrEmpty(monsterName))
        {
            string objName = gameObject.name;
            
            // Strip common prefixes/suffixes from Unity prefab names
            if (objName.EndsWith("(Clone)"))
            {
                objName = objName.Substring(0, objName.Length - 7);
            }
            
            if (objName.Contains(" Variant"))
            {
                objName = objName.Replace(" Variant", "");
            }
            
            monsterName = objName;
            Debug.Log($"Auto-detected monster name: {monsterName}");
        }
    }
    
    private void Update()
    {
        // Skip if already discovered or discovery on sight is disabled
        if (discovered || !discoverOnSight || playerTransform == null) return;
        
        // Check distance to player
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= sightRange)
        {
            DiscoverMonster();
        }
    }
    
    private void OnDestroy()
    {
        // When the monster is destroyed (usually when defeated), discover it in the book
        if (discoverOnDeath && !discovered)
        {
            DiscoverMonster();
        }
    }
    
    public void DiscoverMonster()
    {
        if (discovered) return;
        
        if (bookManager != null)
        {
            bookManager.UnlockMonster(monsterName);
            discovered = true;
        }
    }
    
    // Method to manually trigger discovery from other scripts
    public void ManualDiscover()
    {
        DiscoverMonster();
    }
} 