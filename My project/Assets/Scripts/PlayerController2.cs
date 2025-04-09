using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] public event Action onEncountered;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    public void HandleUpdate()
    {
        if (CheckForBossBattle()) {
            onEncountered();
        }
    }

    public bool CheckForBossBattle(string enemyTag = "BossMonster")
    {
        if (characterController == null)
        {
            return false; // No CharacterController, so can't be hitting anything
        }

        // Get all colliders that the CharacterController is currently overlapping with.
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, characterController.radius, LayerMask.GetMask("Default")); // You might need to adjust the LayerMask

        foreach (Collider hitCollider in hitColliders)
        {
            // Check if the hit collider belongs to a GameObject with the specified enemy tag.
            if (hitCollider.CompareTag(enemyTag) && hitCollider is BoxCollider)
            {
                return true; // Found an enemy's BoxCollider
            }
        }

        return false; // No enemy BoxCollider found in the overlaps
    }
}
