using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class PlayerLocator : MonoBehaviour
{
    // Static list containing all player roots currently in the scene
    public static readonly List<Transform> Players = new List<Transform>();

    void Awake()
    {
        // Register this player when it spawns
        if (!Players.Contains(transform))
            Players.Add(transform);
    }

    void OnDestroy()
    {
        // Remove it when it gets destroyed or scene reloads
        Players.Remove(transform);
    }

    // Optional helper: quick access for first two players
    public static Transform Player1 => Players.Count > 0 ? Players[0] : null;
    public static Transform Player2 => Players.Count > 1 ? Players[1] : null;

    // Optional: find nearest player to a given point
    public static Transform GetNearestPlayer(Vector3 position)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var p in Players)
        {
            if (!p) continue;
            float dist = Vector3.Distance(position, p.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = p;
            }
        }

        return nearest;
    }
}
