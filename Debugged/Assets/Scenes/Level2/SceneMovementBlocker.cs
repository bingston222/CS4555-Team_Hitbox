using UnityEngine;

public class SceneMovementBlocker : MonoBehaviour
{
    public bool blockMovement = false;

    private MonoBehaviour movementScript;  // Your real movement script
    private Animator animator;

    void Awake()
    {
        // Try to find your actual controller
        movementScript = GetComponent<MyCharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (blockMovement)
        {
            if (movementScript != null)
                movementScript.enabled = false;

            if (animator != null)
                animator.SetFloat("Speed", 0f);
        }
        else
        {
            if (movementScript != null)
                movementScript.enabled = true;
        }
    }
}
