using UnityEngine;

public class MeleeAttackBehavior : StateMachineBehaviour
{
    Transform player;
    HealthSystem playerHealth;
    Rigidbody spiderRigidbody;

    public float attackDamage = 10f;
    public float attackInterval = 1f;
    private float lastAttackTime;
    private bool hasAttacked;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<HealthSystem>();
        spiderRigidbody = animator.GetComponent<Rigidbody>();
        lastAttackTime = Time.time - attackInterval;
        hasAttacked = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!player || !playerHealth) return;

        // Only rotate the spider, don't move it
        Vector3 directionToPlayer = (player.position - animator.transform.position).normalized;
        directionToPlayer.y = 0; // Keep the rotation only on the Y axis
        animator.transform.rotation = Quaternion.LookRotation(directionToPlayer);
        
        float distance = Vector3.Distance(animator.transform.position, player.position);

        if (distance > 4)
        {
            animator.SetBool("isMeleeAttacking", false);
        }
        else
        {
            animator.SetBool("isMeleeAttacking", true);

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
