using UnityEngine;

public class BoxGuyController : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool isMoving = Input.GetKey(KeyCode.J);
        animator.SetBool("isWalking", isMoving);
    }
}
