using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Player player { get; private set; }
    public float attackRange = 2f;
    public LayerMask enemyLayers;

    public Animator animator;

    void Update()
    {
        // Left Click, which is the cone -> Spells[0]
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void Attack()
    {
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

        animator.SetTrigger("attack");

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(player.Spells[0]);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}