using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public float attackRange = 5f;       // Distance within which the enemy can be hurt
    public int attackDamage = 20;        // How much damage is dealt
    public LayerMask enemyLayer;         // Layer for enemy detection
    public Transform attackPoint;        // Position from where we check the overlap sphere

    // Audio fields:
    public AudioSource audioSource;      // Assign an AudioSource in the Inspector
    public AudioClip attackSound;        // Assign an attack sound clip in the Inspector

    // Cooldown settings:
    public float attackCooldown = 1f;    // Time in seconds between attacks
    private float lastAttackTime = 0f;   // Tracks the time since last attack
    
    void Update()
    {
        // Check left mouse click for in-range attack, and enforce the cooldown
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            AttemptAttack();
            lastAttackTime = Time.time;
        }

        // Check Y key for direct attack (ignores range), and enforce the cooldown
        if (Input.GetKeyDown(KeyCode.Y) && Time.time >= lastAttackTime + attackCooldown)
        {
            DirectAttack();
            lastAttackTime = Time.time;
        }
    }

    void AttemptAttack()
    {
        // Use OverlapSphere to find enemies within attackRange on enemyLayer
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        // Apply damage to any Enemy component found within range
        foreach (Collider c in hits)
        {
            Enemy enemy = c.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }

        // Play attack sound if assigned
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    void DirectAttack()
    {
        // Get all enemy objects in the scene regardless of position
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            enemy.TakeDamage(attackDamage);
        }

        // Play the attack sound if assigned
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    // Draw a red sphere in the Editor to visualize the attack range
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}