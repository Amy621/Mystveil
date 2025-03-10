using UnityEngine;

public class MeleeAttackBehavior : StateMachineBehaviour
{
    Transform player;
    HealthSystem playerHealth;

    public float attackDamage = 10f;
    public float attackInterval = 1f;
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
                    playerHealth.TakeDamage(attackDamage);
                    lastAttackTime = Time.time;
                    hasAttacked = true;
                }
            }
        }
    }
}
