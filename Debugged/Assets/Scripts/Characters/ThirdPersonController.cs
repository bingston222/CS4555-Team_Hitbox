using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    public float moveSpeed = 5f, sprintSpeed = 8f, jumpForce = 7f, gravity = -20f;
    public float turnSmooth = 12f;
    public Transform cameraPivot; // drag Main Camera here

    CharacterController cc;
    Animator anim;
    Vector3 vel;

    void Awake(){ cc = GetComponent<CharacterController>(); anim = GetComponent<Animator>(); }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(h,0,v).normalized;

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        if (dir.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + (cameraPivot ? cameraPivot.eulerAngles.y : 0f);
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * turnSmooth);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            cc.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (cc.isGrounded){ vel.y = -2f; if (Input.GetKeyDown(KeyCode.Space)) vel.y = jumpForce; }
        vel.y += gravity * Time.deltaTime;
        cc.Move(vel * Time.deltaTime);

        if (anim){ anim.SetFloat("Speed", new Vector2(h,v).magnitude); anim.SetBool("IsGrounded", cc.isGrounded); }
    }
}
