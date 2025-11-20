using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class DoorThroughWatcher : MonoBehaviour
{
    private readonly HashSet<int> inside = new HashSet<int>();
    public bool BothThrough => inside.Count >= 2;

    void OnTriggerEnter(Collider other)
    {
        var pi = other.GetComponentInParent<PlayerInput>();
        if (!pi) return;
        inside.Add(pi.playerIndex); // 0,1
        // Debug.Log($"Through enter P{pi.playerIndex+1} (count {inside.Count})");
    }

    void OnTriggerExit(Collider other)
    {
        var pi = other.GetComponentInParent<PlayerInput>();
        if (!pi) return;
        inside.Remove(pi.playerIndex);
        // Debug.Log($"Through exit P{pi.playerIndex+1} (count {inside.Count})");
    }

    public void ResetCount() => inside.Clear();
}
