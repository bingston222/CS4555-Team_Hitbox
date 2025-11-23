using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f, sprintSpeed = 8f, jumpForce = 7f, gravity = -20f;
    public float turnSmooth = 12f;
    public Transform cameraPivot; // assign your camera rig / follow target

    [Header("Player Keys")]
    public KeyCode up = KeyCode.W, down = KeyCode.S, left = KeyCode.A, right = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space, sprintKey = KeyCode.LeftShift;

    [Header("Animator Param Names")]
    public string speedParam = "Speed";
    public string sprintBool = "isSprinting";
    public string jumpTrigger = "Jump";
    public string jumpingBool = "isJumping";

    CharacterController cc;
    Animator anim;
    Vector3 vel;
    bool isJumping;

    void Awake()
    {
        cc  = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        // CC sanity
        cc.skinWidth = Mathf.Max(cc.skinWidth, 0.05f);
        if (cc.height < 1f) cc.height = 1.8f;
        if (cc.radius < 0.1f) cc.radius = 0.3f;

        vel = Vector3.zero;

        // ensure animator doesn’t try to drive root motion
        //if (anim) anim.applyRootMotion = false;
    }

    void OnEnable()
    {
        vel.y = -2f;
        isJumping = false;

        if (anim)
        {
            SafeResetTrigger(jumpTrigger);
            SafeSetBool(jumpingBool, false);
            // removed old "isWalking" usage (param didn’t exist)
        }
    }

    void Update()
    {
        if (!cc || !cc.enabled) return;

        // Input
        float h = (Input.GetKey(left) ? -1f : 0f) + (Input.GetKey(right) ? 1f : 0f);
        float v = (Input.GetKey(down) ? -1f : 0f) + (Input.GetKey(up) ? 1f : 0f);
        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        float baseSpeed = Input.GetKey(sprintKey) ? sprintSpeed : moveSpeed;

        // Planar move (camera-relative if pivot assigned)
        Vector3 planarMove = Vector3.zero;
        if (inputDir.sqrMagnitude > 0.01f)
        {
            float camY = cameraPivot ? cameraPivot.eulerAngles.y : 0f;
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + camY;
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * turnSmooth);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            planarMove = moveDir * baseSpeed; // m/s
        }

        // Ground / jump
        if (cc.isGrounded)
        {
            if (isJumping)
            {
                isJumping = false;
                SafeResetTrigger(jumpTrigger);
                SafeSetBool(jumpingBool, false);
            }

            vel.y = -2f;
            if (Input.GetKeyDown(jumpKey))
            {
                vel.y = jumpForce;
                isJumping = true;
                SafeResetTrigger(jumpTrigger);
                SafeSetTrigger(jumpTrigger);
                SafeSetBool(jumpingBool, true);
            }
        }

        // Gravity
        vel.y += gravity * Time.deltaTime;

        // One Move
        Vector3 motion = planarMove;
        motion.y = vel.y;
        cc.Move(motion * Time.deltaTime);

        // Animator params
        float horizSpeed = planarMove.magnitude;
        float walkNorm   = Mathf.InverseLerp(0f, moveSpeed, horizSpeed);
        SafeSetFloat(speedParam, walkNorm, 0.1f);
        SafeSetBool(sprintBool, Input.GetKey(sprintKey) && horizSpeed > 0.1f);
    }

    // ---- Safe animator helpers (avoid missing-param warnings) ----
    bool HasParam(string name, AnimatorControllerParameterType type)
    {
        if (!anim) return false;
        foreach (var p in anim.parameters) if (p.type == type && p.name == name) return true;
        return false;
    }
    void SafeSetFloat(string name, float v, float damp = 0f)
    {
        if (anim && HasParam(name, AnimatorControllerParameterType.Float))
            anim.SetFloat(name, v, damp, Time.deltaTime);
    }
    void SafeSetBool(string name, bool v)
    {
        if (anim && HasParam(name, AnimatorControllerParameterType.Bool))
            anim.SetBool(name, v);
    }
    void SafeSetTrigger(string name)
    {
        if (anim && HasParam(name, AnimatorControllerParameterType.Trigger))
            anim.SetTrigger(name);
    }
    void SafeResetTrigger(string name)
    {
        if (anim && HasParam(name, AnimatorControllerParameterType.Trigger))
            anim.ResetTrigger(name);
    }

    void OnDestroy()
    {
        Debug.Log($"[ThirdPersonController] Destroyed: {name}");
    }
}
