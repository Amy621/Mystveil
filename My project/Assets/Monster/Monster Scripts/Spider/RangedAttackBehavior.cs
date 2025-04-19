using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackBehavior : StateMachineBehaviour
{
    // for monster moves
    // overworld monster has max of 2 moves, 1 physical, 1 ranged
    // use the RANGED for this one, which should be index 1 (2nd move)
    public EnemyBase enemy { get; private set; }
    Transform player;
    HealthSystem playerHealth;

    // public float attackDamage = 5f;
    public float attackInterval = 2f;
    private float lastAttackTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>().monster;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<HealthSystem>();
        lastAttackTime = Time.time - attackInterval;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null || playerHealth == null)
            return;

        animator.transform.LookAt(player);

        float distance = Vector3.Distance(animator.transform.position, player.position);
        if (distance > 10)
        {
            animator.SetBool("isRangedAttacking", false);
        }
        if (distance < 4)
        {
            animator.SetBool("isRangedAttacking", false);
            animator.SetBool("isMeleeAttacking", true);
        }
        else if (distance <= 10)
        {
            animator.SetBool("isRangedAttacking", true);

            if (Time.time >= lastAttackTime + attackInterval)
            {
                playerHealth.TakeDamage(enemy, enemy.Moves[1]);
                lastAttackTime = Time.time;
            }
        }
    }
}
