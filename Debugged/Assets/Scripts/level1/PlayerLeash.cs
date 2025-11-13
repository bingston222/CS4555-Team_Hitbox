using UnityEngine;

public class PlayerLeash : MonoBehaviour
{
    public Transform p1;
    public Transform p2;
    public float maxDistance = 10f;
    public float pullForce = 6f;

    void FixedUpdate()
    {
        Vector3 delta = p2.position - p1.position;
        float dist = delta.magnitude;
        if (dist > maxDistance)
        {
            Vector3 pull = delta.normalized * (dist - maxDistance) * pullForce * Time.fixedDeltaTime;
            p1.position += pull;
            p2.position -= pull;
        }
    }
}
