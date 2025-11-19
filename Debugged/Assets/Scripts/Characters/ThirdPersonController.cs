using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f, sprintSpeed = 8f, jumpForce = 7f, gravity = -20f;
    public float turnSmooth = 12f;
    public Transform cameraPivot;

    [Header("Player Keys")]
    public KeyCode up = KeyCode.W, down = KeyCode.S, left = KeyCode.A, right = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space, sprintKey = KeyCode.LeftShift;

    CharacterController cc;
    Animator anim;
    Vector3 vel;
    bool isJumping;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        // Sanity: ensure CC reasonable to avoid instant overlaps
        cc.skinWidth = Mathf.Max(cc.skinWidth, 0.05f);
        if (cc.height < 1f) cc.height = 1.8f;
        if (cc.radius < 0.1f) cc.radius = 0.3f;

        // Start grounded-ish
        vel = Vector3.zero;
    }

    void OnEnable()
    {
        // Reset vertical velocity when (re)spawning
        vel.y = -2f;
        isJumping = false;
        if (anim)
        {
            anim.ResetTrigger("Jump");
            anim.SetBool("isJumping", false);
            anim.SetBool("isWalking", false);
        }
    }

    void Update()
{
    if (!cc || !cc.enabled) return;

    float h = (Input.GetKey(left) ? -1f : 0f) + (Input.GetKey(right) ? 1f : 0f);
    float v = (Input.GetKey(down) ? -1f : 0f) + (Input.GetKey(up) ? 1f : 0f);
    Vector3 inputDir = new Vector3(h, 0f, v).normalized;

    float baseSpeed = Input.GetKey(sprintKey) ? sprintSpeed : moveSpeed;

    // --- Planar move (XZ) ---
    Vector3 planarMove = Vector3.zero;
    if (inputDir.sqrMagnitude > 0.01f)
    {
        float camY = cameraPivot ? cameraPivot.eulerAngles.y : 0f;
        float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + camY;
        float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * turnSmooth);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        planarMove = moveDir * baseSpeed;          // <-- store horizontal motion (m/s)
    }

    // --- Ground / jump ---
    if (cc.isGrounded)
    {
        if (isJumping)
        {
            isJumping = false;
            anim.ResetTrigger("Jump");
            anim.SetBool("isJumping", false);
        }

        vel.y = -2f; // stick
        if (Input.GetKeyDown(jumpKey))
        {
            vel.y = jumpForce;
            isJumping = true;
            anim.ResetTrigger("Jump");
            anim.SetTrigger("Jump");
            anim.SetBool("isJumping", true);
        }
    }

    // --- Gravity ---
    vel.y += gravity * Time.deltaTime;

    // --- Single Move call ---
    Vector3 motion = planarMove;
    motion.y = vel.y;
    cc.Move(motion * Time.deltaTime);

    // --- Animator params (use planarMove magnitude, not cc.velocity after a second Move) ---
    float horizSpeed = planarMove.magnitude;                   // m/s
    float walkNorm   = Mathf.InverseLerp(0f, moveSpeed, horizSpeed); // 0..1 (0 = idle, 1 = walk)
    anim.SetFloat("Speed", walkNorm, 0.1f, Time.deltaTime);

    bool sprinting = Input.GetKey(sprintKey) && horizSpeed > 0.1f;
    anim.SetBool("isSprinting", sprinting);
}


    void OnDestroy()
    {
        Debug.Log($"[ThirdPersonController] Destroyed: {name}");
     Debug.LogWarning($"[ThirdPersonController] Destroyed: {name}\n{System.Environment.StackTrace}");


    }
}
