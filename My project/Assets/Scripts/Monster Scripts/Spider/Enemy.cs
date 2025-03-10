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

    public void TakeDamage(int damageAmount) 
    {
        Debug.Log("In Take Damage!!!");
        enemyHP -= damageAmount;

        Debug.Log("Current health: " + enemyHP);

        if (enemyHP <= 0)
        {
            //Play Death Animation
            animator.SetTrigger("death");
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        }
        else
        {
            //Play Damage Animation
            animator.SetTrigger("damage");
        }
    }
}
