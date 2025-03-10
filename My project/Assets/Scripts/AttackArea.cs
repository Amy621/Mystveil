using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private int damageAmount = 20;

    private void onTriggerEnter(Collider other)
    {
        Debug.Log("inside the trigger function!");
        if (other.GetComponent<Enemy>() != null)
        {
            Enemy e = other.GetComponent<Enemy>();
            if(e != null)
            {
                e.TakeDamage(damageAmount);
                return;
            }
        }
    }
}
