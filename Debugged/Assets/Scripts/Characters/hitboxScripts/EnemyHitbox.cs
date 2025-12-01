using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private Collider col;
    private Vector3 origSize;
    private float origRadius;
    private bool isSphere, isBox, isCapsule;

    void Awake()
    {
        col = GetComponent<Collider>();
        if (!col)
        {
            Debug.LogWarning($"{name} has no collider for EnemyHitbox to modify!");
            return;
        }

        isSphere  = col is SphereCollider;
        isBox     = col is BoxCollider;
        isCapsule = col is CapsuleCollider;

        if (isSphere)
            origRadius = ((SphereCollider)col).radius;
        if (isCapsule)
            origRadius = ((CapsuleCollider)col).radius;
        if (isBox)
            origSize = ((BoxCollider)col).size;
    }

    // Makes the collider larger by a given multiplier
    public void Enlarge(float multiplier)
    {
        if (!col) return;

        if (isSphere)
            ((SphereCollider)col).radius = origRadius * multiplier;
        if (isCapsule)
            ((CapsuleCollider)col).radius = origRadius * multiplier;
        if (isBox)
            ((BoxCollider)col).size = origSize * multiplier;
    }

    // Restores collider to its original dimensions
    public void Restore()
    {
        if (!col) return;

        if (isSphere)
            ((SphereCollider)col).radius = origRadius;
        if (isCapsule)
            ((CapsuleCollider)col).radius = origRadius;
        if (isBox)
            ((BoxCollider)col).size = origSize;
    }
}
