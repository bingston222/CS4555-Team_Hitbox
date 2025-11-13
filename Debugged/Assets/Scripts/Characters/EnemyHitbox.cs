using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    Collider col; Vector3 origSize; float origRadius; bool sphere, box, capsule;

    void Awake()
    {
        col = GetComponent<Collider>();
        sphere  = col is SphereCollider;
        box     = col is BoxCollider;
        capsule = col is CapsuleCollider;

        if (sphere)  origRadius = ((SphereCollider)col).radius;
        if (capsule) origRadius = ((CapsuleCollider)col).radius;
        if (box)     origSize   = ((BoxCollider)col).size;
    }

    public void Enlarge(float f)
    {
        if (sphere)  ((SphereCollider)col).radius  = origRadius * f;
        if (capsule) ((CapsuleCollider)col).radius = origRadius * f;
        if (box)     ((BoxCollider)col).size       = origSize * f;
    }
    public void Restore()
    {
        if (sphere)  ((SphereCollider)col).radius  = origRadius;
        if (capsule) ((CapsuleCollider)col).radius = origRadius;
        if (box)     ((BoxCollider)col).size       = origSize;
    }
}
