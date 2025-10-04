using UnityEngine;

public class Interactor : MonoBehaviour
{
    public float range = 2.0f;
    public KeyCode useKey = KeyCode.E;
    public LayerMask interactableMask = ~0;

    PlayerId myId;

    void Awake() { myId = GetComponent<PlayerId>(); }

    void Update()
    {
        var (it, distSq, _) = FindNearestWithDistance();
        if (it != null)
        {
            string keyLabel = useKey.ToString();

            // Replace any long key names with symbols
            if (keyLabel == "Slash") keyLabel = "/";
            if (keyLabel == "LeftShift") keyLabel = "L-Shift";
            if (keyLabel == "RightShift") keyLabel = "R-Shift";

            string prompt = (it is SpotInteractable)
                ? $"Search ({keyLabel})"
                : it.GetPromptText();

            InteractionPromptUI.Request(prompt, distSq);

            if (Input.GetKeyDown(useKey))
                it.Interact(this);
        }
    }


    (IInteractable, float, Collider) FindNearestWithDistance()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, interactableMask);
        float best = float.MaxValue;
        IInteractable bestIt = null;
        Collider bestCol = null;

        foreach (var h in hits)
        {
            IInteractable it = h.GetComponent<IInteractable>()
                               ?? h.GetComponentInParent<IInteractable>()
                               ?? h.GetComponentInChildren<IInteractable>();
            if (it == null) continue;

            Vector3 p = h.ClosestPoint(transform.position);
            float d = (p - transform.position).sqrMagnitude;

            if (d < best)
            {
                best = d;
                bestIt = it;
                bestCol = h;
            }
        }
        return (bestIt, best, bestCol);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
