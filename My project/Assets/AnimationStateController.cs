using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    int is_RunningHash;

    float velocity = 0.0f;

    public float acceleration = 0.1f;
    int VelocityHash;
    void Start()
    {
        animator = GetComponent<Animator>();
        is_RunningHash = Animator.StringToHash("is_Running");
        VelocityHash = Animator.StringToHash("Velocity");

    }

    // Update is called once per frame
    void Update()
    {
        bool is_Running = animator.GetBool("is_Running");
        bool forwardPressed = Input.GetKey("w");
         bool sidePressed = Input.GetKey("s") || Input.GetKey("d") || Input.GetKey("a"); // Check for 'S' or 'D'
      if (!is_Running && (forwardPressed || sidePressed))
        {
            velocity += Time.deltaTime * acceleration;
            animator.SetBool("is_Running", true);
        }
        if(is_Running && (!forwardPressed && !sidePressed))
        {
            animator.SetBool("is_Running", false);
        }

        animator.SetFloat(VelocityHash, velocity);
        
    }
}


