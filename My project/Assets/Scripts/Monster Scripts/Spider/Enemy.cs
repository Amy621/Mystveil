using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyHP = 100;

    public GameObject projectile;
    public Transform projectilePoint;
    public float projectileDisappearDelay;

    public GameObject clawmark;
    public Transform clawmarkPoint;
    public float clawDisappearDelay;

    public Animator animator;

    public void Shoot() 
    {
        GameObject instantiatedProjectile = Instantiate(projectile, projectilePoint.position, Quaternion.identity);
        Rigidbody rb = instantiatedProjectile.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward *10f, ForceMode.Impulse);
        rb.AddForce(transform.up *12f, ForceMode.Impulse);

        StartCoroutine(DisappearProjectile(instantiatedProjectile, projectileDisappearDelay));
    }

    private IEnumerator DisappearProjectile(GameObject projectileObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (projectileObject != null)
        {
            Destroy(projectileObject);
        }
    }

    public void Claw() 
    {
        GameObject instantiatedClawmark = Instantiate(clawmark, clawmarkPoint.position, clawmarkPoint.rotation);
        StartCoroutine(DisappearClawmark(instantiatedClawmark, clawDisappearDelay));
    }

    private IEnumerator DisappearClawmark(GameObject clawmarkObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (clawmarkObject != null)
        {
            Destroy(clawmarkObject);
        }
    }

    [Header("Animation Triggers")]
    public string deathTriggerName = "death";
    public string damageTriggerName = "damage";

    public void TakeDamage(int damageAmount) 
    {
        enemyHP -= damageAmount;
        Debug.Log("Current health: " + enemyHP);

        if (enemyHP <= 0)
        {
            animator.SetTrigger(deathTriggerName);
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        }
        else
        {
            animator.SetTrigger(damageTriggerName);
        }
    }
}
