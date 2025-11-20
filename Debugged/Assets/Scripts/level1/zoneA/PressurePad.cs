using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class PressurePad: MonoBehaviour
{
    // Which players are currently on this pad
    private readonly HashSet<int> playersOnPad = new HashSet<int>();

    public bool IsOccupied => playersOnPad.Count > 0;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // Helps guarantee trigger messages fire (even with CharacterController)
        if (!TryGetComponent<Rigidbody>(out var rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

 void OnTriggerEnter(Collider other)
{
    if (TryGetPlayerIndex(other, out int idx))
        playersOnPad.Add(idx);
}

void OnTriggerStay(Collider other)   // <--- add this
{
    if (TryGetPlayerIndex(other, out int idx))
        playersOnPad.Add(idx);
    // inside OnTriggerStay:
Debug.Log($"[{name}] STAY (players:{playersOnPad.Count})", this);

}

void OnTriggerExit(Collider other)
{
    if (TryGetPlayerIndex(other, out int idx))
        playersOnPad.Remove(idx);
}

    bool TryGetPlayerIndex(Collider c, out int indexZeroBased)
{
    indexZeroBased = 0;

    // New simple tag-based check:
    if (c.CompareTag("Player"))
        return true;

    return false;
}

}
