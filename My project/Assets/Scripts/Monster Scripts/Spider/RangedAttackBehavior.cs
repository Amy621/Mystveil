using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackBehavior : StateMachineBehaviour
{
    Transform player;
    HealthSystem playerHealth;

    public float attackDamage = 5f;
    public float attackInterval = 2f;
    private float lastAttackTime;
    private bool hasAttacked;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<HealthSystem>();
        lastAttackTime = Time.time - attackInterval;
        hasAttacked = false;
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

            // Only deal damage once per attack animation when we reach the halfway point
            if (!hasAttacked && stateInfo.normalizedTime >= 0.5f && stateInfo.normalizedTime < 0.6f)
            {
                if (Time.time >= lastAttackTime + attackInterval)
                {
                    playerHealth.TakeDamage(attackDamage);
                    lastAttackTime = Time.time;
                    hasAttacked = true;
                }
            }
        }
    }
}
