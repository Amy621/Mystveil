using UnityEngine;

public class MeleeAttackBehavior : StateMachineBehaviour
{
    // for monster moves
    // overworld monster has max of 2 moves, 1 physical, 1 ranged
    // use the RANGED for this one, which should be index 1 (2nd move)
    public EnemyBase enemy { get; private set; }
    Transform player;
    HealthSystem playerHealth;

    // public float attackDamage = 10f;
    public float attackInterval = 1f;
    private float lastAttackTime;
    private bool hasAttacked;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>().monster;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<HealthSystem>();
        lastAttackTime = Time.time - attackInterval;
        hasAttacked = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!player || !playerHealth) return;

        animator.transform.LookAt(player);
        float distance = Vector3.Distance(animator.transform.position, player.position);

        if (distance > 4)
        {
            animator.SetBool("isMeleeAttacking", false);
        }
        else
        {
            animator.SetBool("isMeleeAttacking", true);

            if (!hasAttacked && stateInfo.normalizedTime % 1 >= 0.5f)
            {
                if (Time.time >= lastAttackTime + attackInterval)
                {
                    playerHealth.TakeDamage(enemy, enemy.Moves[0]);
                    lastAttackTime = Time.time;
                    hasAttacked = true;
                }
            }
        }
    }
}
