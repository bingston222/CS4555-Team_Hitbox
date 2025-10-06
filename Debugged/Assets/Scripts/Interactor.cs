using UnityEngine;

public class Interactor : MonoBehaviour
{
    public float range = 2.0f;
    public KeyCode useKey = KeyCode.E;
    public LayerMask interactableMask = ~0;

    private PlayerId myId;

    void Awake() { myId = GetComponent<PlayerId>(); }

    void Update()
    {
        var (it, distSq, _) = FindNearestWithDistance();
        if (it == null) return;

        // ---- STOP SEARCH PROMPTS AFTER BOTH CONTROLLERS ARE FOUND ----
        // Still allow other interactables (e.g., plugs/buttons), but
        // skip SpotInteractable so "Search (E/ /)" never appears again.
        if (it is SpotInteractable && GameState.I != null
            && GameState.I.P1HasController && GameState.I.P2HasController)
        {
            return; // no prompt, no interact
        }

        // Build the key label
        string keyLabel = useKey.ToString();
        if (keyLabel == "Slash") keyLabel = "/";
        if (keyLabel == "LeftShift")  keyLabel = "L-Shift";
        if (keyLabel == "RightShift") keyLabel = "R-Shift";

        // Let interactable override; otherwise show "Search (key)" for spots
        string prompt = (it is SpotInteractable)
            ? $"Search ({keyLabel})"
            : it.GetPromptText();

        // Send prompt (closest-wins arbitration is inside the UI script)
        InteractionPromptUI.Request(prompt, distSq);

        // Interact
        if (Input.GetKeyDown(useKey))
            it.Interact(this);
    }

    // ---------------------------------------------------------------

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
