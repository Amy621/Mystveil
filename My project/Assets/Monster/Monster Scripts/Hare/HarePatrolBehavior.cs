using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarePatrolBehavior : StateMachineBehaviour
{
    float timer;
    List<Transform> wayPoints = new List<Transform>();
    UnityEngine.AI.NavMeshAgent agent;

    Transform player;
    float RunRange = 6;
    Transform currentWaypoint;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        Transform wayPointsObject = GameObject.FindGameObjectWithTag("WayPoints").transform;
        foreach(Transform t in wayPointsObject) {
            wayPoints.Add(t);
        }

        agent = animator.GetComponent<UnityEngine.AI.NavMeshAgent>();
        GoToNewWaypoint(animator);

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(agent.remainingDistance <= agent.stoppingDistance) {
            GoToNewWaypoint(animator);
        }

        timer += Time.deltaTime;
        if (timer > Random.Range(5, 25)) {
            animator.SetBool("isPatrolling", false);
        }

        float distance = Vector3.Distance(animator.transform.position, player.position);
        if (distance < RunRange) {
            animator.SetBool("isRunning", true);
        }

       // CheckForSpiderCollision(animator);
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       agent.SetDestination(agent.transform.position);
    }

    void CheckForSpiderCollision(Animator animator)
    {
        // Get the BoxCollider of the current spider.
        BoxCollider boxCollider = animator.GetComponent<BoxCollider>();

        if (boxCollider == null)
        {
            Debug.LogError("Spider does not have a BoxCollider!");
            return;
        }

        // Get the size and center of the BoxCollider.
        Vector3 boxSize = boxCollider.size;
        Vector3 boxCenter = boxCollider.center;

        // Get the transform of the current spider.
        Transform spiderTransform = animator.transform;

        // Calculate the world-space center of the box.
        Vector3 worldCenter = spiderTransform.TransformPoint(boxCenter);

        // Calculate the world-space rotation of the box.
        Quaternion worldRotation = spiderTransform.rotation;

        // Use OverlapBox to detect collisions.
        Collider[] hitColliders = Physics.OverlapBox(worldCenter, boxSize / 2f, worldRotation); // Divide by 2, as OverlapBox uses half extents.

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster") && hitCollider.gameObject != animator.gameObject)
            {
                // Another spider is detected. Turn around.
                Debug.Log("Another monster detected!");
                GoToNewWaypoint(animator);
                break; // Stop checking after turning around.
            }
        }
    }

    void GoToNewWaypoint(Animator animator)
    {
        if (wayPoints.Count > 0)
        {
            Transform newWaypoint;
            do
            {
                int randomIndex = Random.Range(0, wayPoints.Count - 1);
                newWaypoint = wayPoints[randomIndex];
            } while (newWaypoint == currentWaypoint); // Ensure a different waypoint is chosen.

            currentWaypoint = newWaypoint; // Update the current waypoint.
            agent.SetDestination(currentWaypoint.position);
        }
        else
        {
            Debug.LogWarning("No waypoints available!");
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
