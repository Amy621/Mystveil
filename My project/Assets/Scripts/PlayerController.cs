using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // [SerializeField] private float _speed = 1;
    // [SerializeField] private float _jumpForce = 200;
    // [SerializeField] private Rigidbody _rb;
    // [SerializeField] private float _groundCheckDistance = 1.1f;
    [SerializeField] private CharacterController characterController;
    [SerializeField] public event Action onEncountered;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        // _rb = GetComponent<Rigidbody>();
    }
    public void HandleUpdate()
    {
        // var vel = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * _speed;
        // vel.y = _rb.velocity.y;
        // _rb.velocity = vel;

        // if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        // {
        //     _rb.AddForce(Vector3.up * _jumpForce);
        // }

        if (CheckForBossBattle()) {
            onEncountered();
        }
    }

    // private bool IsGrounded()
    // {
    //     return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance);
    // }

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
