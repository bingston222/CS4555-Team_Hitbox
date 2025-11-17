using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f, sprintSpeed = 8f, jumpForce = 7f, gravity = -20f;
    public float turnSmooth = 12f;
    public Transform cameraPivot;

    [Header("Player Keys")]
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    CharacterController cc;
    Animator anim;
    Vector3 vel;
    bool isJumping = false;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!cc || !cc.enabled) return;

        float h = (Input.GetKey(left) ? -1f : 0f) + (Input.GetKey(right) ? 1f : 0f);
        float v = (Input.GetKey(down) ? -1f : 0f) + (Input.GetKey(up) ? 1f : 0f);
        Vector3 dir = new Vector3(h, 0, v).normalized;

        float baseSpeed = Input.GetKey(sprintKey) ? sprintSpeed : moveSpeed;
        float finalSpeed = baseSpeed;

        // --- horizontal movement ---
        if (dir.sqrMagnitude > 0.01f)
        {
            float camY = cameraPivot ? cameraPivot.eulerAngles.y : 0f;
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + camY;
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * turnSmooth);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cc.Move(moveDir * finalSpeed * Time.deltaTime);
        }

        // --- vertical movement (gravity & jump) ---
        // --- vertical: ground + jump + gravity ---
if (cc.isGrounded)
{
    // 👇 this part ensures jump animation resets after landing
    if (isJumping)
    {
        isJumping = false;
        anim.ResetTrigger("Jump");
        anim.SetBool("isJumping", false);
        anim.CrossFade("idle", 0.15f, 0);   // forces back to idle animation
    }

    vel.y = -2f; // stick to ground

    if (Input.GetKeyDown(jumpKey))
    {
        vel.y = jumpForce;
        isJumping = true;
        anim.ResetTrigger("Jump");
        anim.SetTrigger("Jump");
        anim.SetBool("isJumping", true);
    }
}


        vel.y += gravity * Time.deltaTime;
        cc.Move(vel * Time.deltaTime);

        // --- animator parameters for walk/idle ---
        if (anim)
        {
            bool moving = dir.sqrMagnitude > 0.01f && cc.isGrounded;
            anim.SetBool("isWalking", moving);

            // optional for blending later
            float inputMag = new Vector2(h, v).magnitude;
            anim.SetFloat("Speed", inputMag, 0.1f, Time.deltaTime);
        }
    }
}
