using UnityEngine;

public class MoveProbe : MonoBehaviour
{
    CharacterController cc;
    void Awake() { cc = GetComponent<CharacterController>(); }
    void Update()
    {
        if (!cc) return;
        // shows whether the controller is actually moving you
        Debug.Log($"CC enabled:{cc.enabled} grounded:{cc.isGrounded} vel:{cc.velocity.magnitude:0.00}");
    }
}
