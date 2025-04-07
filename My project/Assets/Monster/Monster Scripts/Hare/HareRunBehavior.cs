using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HareRunBehavior : StateMachineBehaviour
{
    UnityEngine.AI.NavMeshAgent agent;
    Transform player;
    float distanceFromPlayer = 8f; // You can adjust this value in the Inspector
    float runSpeed = 6f; // Adjust the running speed as needed
    float stopSpeed = 5f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<UnityEngine.AI.NavMeshAgent>();
        // Ensure there's at least one GameObject tagged "Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            player = players[0].transform;
        }
        else
        {
            Debug.LogError("No GameObject tagged 'Player' found in the scene. HareRunBehavior will not function correctly.");
            animator.SetBool("isRunning", false); // Exit this state if no player is found
            return;
        }

        if (agent != null)
        {
            agent.speed = runSpeed; // Set the running speed when entering the state
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Ensure the player reference is still valid
        if (player == null) return;

        float distance = Vector3.Distance(animator.transform.position, player.position);

        if (distance < distanceFromPlayer)
        {
            // Calculate the direction away from the player
            Vector3 awayDirection = (animator.transform.position - player.position).normalized;
            // Calculate the target position to run towards
            Vector3 runToPosition = animator.transform.position + awayDirection * distanceFromPlayer * 2f; // Run a bit further

            if (agent != null && agent.isOnNavMesh)
            {
                agent.SetDestination(runToPosition);
                animator.SetBool("isRunning", true);
                agent.speed = runSpeed;
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
            if (agent != null)
            {
                agent.speed = stopSpeed; // Optionally stop the agent or set a lower speed
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Optionally stop the agent when exiting the run state
        if (agent != null)
        {
            agent.speed = stopSpeed;
            agent.isStopped = false; // Ensure the agent can move again in other states
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
