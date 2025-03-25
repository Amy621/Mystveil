using UnityEngine;
using System.Collections.Generic;

public class EnemyTargeting : MonoBehaviour
{
    [Header("Targeting Settings")]
    public float targetRange = 10f; // Maximum distance to detect enemies
    public float targetSwitchRange = 5f; // Maximum distance to switch targets
    public LayerMask enemyLayer; // Layer for enemy detection
    
    [Header("Visual Feedback")]
    public GameObject targetIndicator; // Visual indicator for the current target
    public Color targetIndicatorColor = Color.red;
    
    private Transform currentTarget;
    private List<Transform> nearbyEnemies = new List<Transform>();
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
        if (targetIndicator != null)
        {
            targetIndicator.SetActive(false);
        }
    }
    
    private void Update()
    {
        // Find all enemies in range
        FindNearbyEnemies();
        
        // Handle target switching with right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            SwitchTarget();
        }
        
        // Update target indicator
        UpdateTargetIndicator();
    }
    
    private void FindNearbyEnemies()
    {
        nearbyEnemies.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, targetRange, enemyLayer);
        
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Monster"))
            {
                nearbyEnemies.Add(col.transform);
            }
        }
    }
    
    private void SwitchTarget()
    {
        if (nearbyEnemies.Count == 0)
        {
            currentTarget = null;
            return;
        }
        
        // If no current target, select the closest enemy
        if (currentTarget == null)
        {
            currentTarget = GetClosestEnemy();
            return;
        }
        
        // Find the next closest enemy
        Transform nextTarget = null;
        float closestDistance = float.MaxValue;
        
        foreach (Transform enemy in nearbyEnemies)
        {
            if (enemy != currentTarget)
            {
                float distance = Vector3.Distance(transform.position, enemy.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nextTarget = enemy;
                }
            }
        }
        
        // Switch to the next target if found
        if (nextTarget != null)
        {
            currentTarget = nextTarget;
        }
    }
    
    private Transform GetClosestEnemy()
    {
        Transform closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (Transform enemy in nearbyEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = enemy;
            }
        }
        
        return closest;
    }
    
    private void UpdateTargetIndicator()
    {
        if (targetIndicator == null) return;
        
        if (currentTarget != null)
        {
            targetIndicator.SetActive(true);
            targetIndicator.transform.position = currentTarget.position;
            
            // Make the indicator face the camera
            targetIndicator.transform.LookAt(mainCamera.transform);
        }
        else
        {
            targetIndicator.SetActive(false);
        }
    }
} 