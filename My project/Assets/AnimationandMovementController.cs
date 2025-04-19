using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationandMovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;

    float rotationFactorPerFrame = 15.0f;
   [SerializeField] float movementSpeed = 6.0f; 
[SerializeField] float gravity = -9.81f;


    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y; // invert the Y-axis to fix forward/backward swap
        // currentMovement.z = -currentMovement.z;

        isMovementPressed = currentMovementInput.magnitude > 0.1f; // check if movement input is significant
    }

    void handleRotation()
    {
        if (isMovementPressed)
        {
            Vector3 positionToLookAt = new Vector3(currentMovement.x, 0.0f, currentMovement.z);
            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(-positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void handleAnimation()
    {
        bool is_Running = animator.GetBool("is_Running");

        if (isMovementPressed && !is_Running)
        {
            animator.SetBool("is_Running", true);
        }
        else if (!isMovementPressed && is_Running)
        {
            animator.SetBool("is_Running", false);
        }
    }

    void Update()
    {
        handleRotation();
        handleAnimation();

        // apply gravity
        if (!characterController.isGrounded)
        {
            currentMovement.y += gravity * Time.deltaTime;
        }

        // apply movement with speed
        characterController.Move(currentMovement.normalized * movementSpeed * Time.deltaTime);
    }

    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
