using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class KeyboardMover : MonoBehaviour
{
    [Header("Input")]
    // Drag P1/Move or P2/Move from your Input Actions asset into this
    public InputActionReference moveAction;

    [Header("Movement")]
    public float moveSpeed = 2.0f;
    public float rotationSpeed = 720f; // deg/sec
    public bool cameraRelative = true;

    private CharacterController controller;
    private Animator animator;
    private Transform cam;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cam = Camera.main ? Camera.main.transform : null;
    }

    void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
    }

    void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
    }

    void Update()
    {
        Vector2 input = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;

        // Convert input to world-space XZ
        Vector3 dir = new Vector3(input.x, 0f, input.y);
        if (cameraRelative && cam != null)
        {
            Vector3 fwd = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cam.right,   Vector3.up).normalized;
            dir = right * dir.x + fwd * dir.z;
        }
        dir = Vector3.ClampMagnitude(dir, 1f);

        // Move & simple gravity
        Vector3 velocity = dir * moveSpeed;
        if (!controller.isGrounded) velocity.y += Physics.gravity.y * Time.deltaTime * 4f;
        controller.Move(velocity * Time.deltaTime);

        // Face movement direction
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Drive Animator blend tree (0 idle → 1 walk)
        if (animator) animator.SetFloat("Speed", new Vector2(input.x, input.y).magnitude);
    }
}
