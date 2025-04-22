using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayer : MonoBehaviour
{

    public HealthSystem healthSystem;
    
    void Start()
    {
        healthSystem = FindObjectOfType<HealthSystem>();

        healthSystem.HealDamage(healthSystem.maxHitPoint);
        healthSystem.RestoreMana(healthSystem.maxManaPoint);
    }
}
