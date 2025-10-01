using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerPusher : MonoBehaviour
{
    [Header("Push Settings")]
    public float pushPower = 3.5f;        // impulse strength
    public float maxPushSpeed = 6f;       // don’t accelerate crates too much
    public LayerMask pushableLayers = ~0; // default: everything

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.rigidbody;
        if (rb == null || rb.isKinematic) return;

        // respect layer mask
        if ((pushableLayers.value & (1 << hit.gameObject.layer)) == 0) return;

        // ignore vertical hits (like standing on top of the box)
        if (hit.moveDirection.y < -0.3f) return;

        // push direction
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z).normalized;

        // cap velocity
        Vector3 horizVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizVel.magnitude < maxPushSpeed)
        {
            rb.AddForce(pushDir * pushPower, ForceMode.Impulse);
        }
    }
}
