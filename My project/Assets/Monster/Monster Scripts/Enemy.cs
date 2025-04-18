using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public EnemyBase monster { get; set; }

    // the only thing you need this for is the spell's attack
    public Player player { get; private set; }

    [Header("Health Settings")]
    public int maxHP;
    private int currentHP;

    [Header("Projectile Settings")]
    public GameObject projectile;
    public Transform projectilePoint;
    public float projectileDisappearDelay = 3f;

    [Header("Claw Settings")]
    public GameObject clawmark;
    public Transform clawmarkPoint;
    public float clawDisappearDelay = 2f;

    [Header("Animation Settings")]
    public Animator animator; 

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip damageClip;
    public AudioClip deathClip;   // Enemy death sound

    [Header("Death Effect Settings")]
    public GameObject deathPrefab;  // Explosion prefab asset
    // Separate explosion sound (the deathPrefab sound)
    public AudioClip explosionClip; 

    private Collider enemyCollider;
    private NavMeshAgent navAgent;

    void Start()
    {
        maxHP = monster.HP;
        Debug.Log("this monster has this much HP: " + maxHP);

        currentHP = maxHP;
        enemyCollider = GetComponent<Collider>();
        navAgent = GetComponent<NavMeshAgent>();

        // get player DB stuff
        PlayerDB playerDB = FindObjectOfType<PlayerDB>();

        if (playerDB != null)
        {
            player = playerDB.Player;
        }
        else
        {
            Debug.LogError("PlayerDB not found in the scene. PlayerUnit cannot be set up.");
            enabled = false;
        }
    }

    void Update()
    {
        // For testing purposes: press F to damage the enemy.
        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     TakeDamage(20);
        // }
    }

    public void Shoot()
    {
        GameObject instantiatedProjectile = Instantiate(projectile, projectilePoint.position, Quaternion.identity);
        Rigidbody rb = instantiatedProjectile.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
        rb.AddForce(transform.up * 12f, ForceMode.Impulse);
        instantiatedProjectile.transform.SetParent(this.transform);
        StartCoroutine(DisappearProjectile(instantiatedProjectile, projectileDisappearDelay));
    }

    IEnumerator DisappearProjectile(GameObject projectileObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (projectileObject) Destroy(projectileObject);
    }

    public void Claw()
    {
        GameObject instantiatedClawmark = Instantiate(clawmark, clawmarkPoint.position, clawmarkPoint.rotation);
        instantiatedClawmark.transform.SetParent(this.transform);
        StartCoroutine(DisappearClawmark(instantiatedClawmark, clawDisappearDelay));
    }

    IEnumerator DisappearClawmark(GameObject clawmarkObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (clawmarkObject) Destroy(clawmarkObject);
    }

    public void TakeDamage(PlayerSpell spell)
    {
        Debug.Log("Enemy taking damage!");

        // adding in formula for damage calculation of monster
        float attack = (spell.Base.Category == MoveCategory.Special)? player.SpAttack : player.Attack;
        float defense = (spell.Base.Category == MoveCategory.Special)? monster.SpDefense : monster.Defense;

        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * player.Level + 10) / 250f;
        float d = a * spell.Base.Power * ((float) attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        Debug.Log("Monster took: " + damage + " damage");

        currentHP -= damage;

        // Play damage sound if assigned
        if (damageClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageClip);
        }

        if (currentHP <= 0)
        {
            // Trigger death animation
            if (animator != null)
            {
                animator.SetTrigger("death");
            }
            // Disable movement and collision
            if (navAgent != null)
            {
                navAgent.enabled = false;
            }
            if (enemyCollider != null)
            {
                enemyCollider.enabled = false;
            }

            // First, play the enemy's death sound immediately
            if (audioSource != null && deathClip != null)
            {
                audioSource.PlayOneShot(deathClip);
            }

            // Then, spawn the explosion effect (and its sound) after a short delay
            StartCoroutine(SpawnExplosion());

            // Destroy the enemy after a delay (e.g., 3 seconds) 
            Destroy(gameObject, 3f);

            // Add a gaining exp function
            int expYield = monster.Base.ExpYield;
            int enemyLevel = monster.Level;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel) / 7);
            player.Exp += expGain;
            Debug.Log("player new exp: " + player.Exp);
            Debug.Log("player level: " + player.Level);
        }
        else
        {
            // Trigger damage animation if still alive
            if (animator != null)
            {
                animator.SetTrigger("damage");
            }
        }
    }

    IEnumerator SpawnExplosion()
    {
        // Wait a brief moment so the death sound has time to play first
        yield return new WaitForSeconds(2.7f);

        if (deathPrefab != null)
        {
            GameObject effectInstance = Instantiate(deathPrefab, transform.position, transform.rotation);
            // Play the explosion sound (if assigned) at the explosion time
            if (audioSource != null && explosionClip != null)
            {
                audioSource.PlayOneShot(explosionClip);
            }
            // Destroy the explosion effect after some time (e.g., 3 seconds)
            Destroy(effectInstance, 3f);
        }
    }
}