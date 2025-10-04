using System.Collections.Generic;
using UnityEngine;

public class SpawnControllers : MonoBehaviour
{
    [Header("Scene references")]
    public Transform hidingSpotsParent;      // parent with children: UnderBed, OnTable, etc.
    public GameObject p1ControllerPrefab;    // kept for instant-spawn/FX if you want
    public GameObject p2ControllerPrefab;

    [Header("Optional: avoid exact same spots as last run")]
    public bool avoidRepeatFromLastPlay = true;

    List<Transform> spots = new List<Transform>();

    void Start()
    {
        if (hidingSpotsParent == null)
        {
            Debug.LogError("[SpawnControllers] HidingSpots parent not assigned.");
            return;
        }

        spots.Clear();
        foreach (Transform t in hidingSpotsParent)
            spots.Add(t);

        if (spots.Count < 2)
        {
            Debug.LogError("[SpawnControllers] Need at least 2 hiding spots.");
            return;
        }

        // ensure a fresh state each Play
        GameState.I.ResetState();

        // Seed is already time-based, but you can uncomment this if you ever set a seed elsewhere:
        // Random.InitState(System.Environment.TickCount);

        // Pick two different indices
        int a = Random.Range(0, spots.Count);
        int b;
        do { b = Random.Range(0, spots.Count); } while (b == a);

        // Optionally avoid repeating exactly last run's spots
        if (avoidRepeatFromLastPlay)
        {
            int lastA = PlayerPrefs.GetInt("last_p1_spot", -1);
            int lastB = PlayerPrefs.GetInt("last_p2_spot", -1);

            // If both match previous run, re-roll once (good enough for intro scene)
            if ((a == lastA && b == lastB) || (a == lastB && b == lastA))
            {
                a = Random.Range(0, spots.Count);
                do { b = Random.Range(0, spots.Count); } while (b == a);
            }

            // Save for next Play
            PlayerPrefs.SetInt("last_p1_spot", a);
            PlayerPrefs.SetInt("last_p2_spot", b);
            PlayerPrefs.Save();
        }

        GameState.I.AssignSpots(a, b);

        Debug.Log($"[SpawnControllers] P1 -> {spots[a].name} (#{a}) | P2 -> {spots[b].name} (#{b})");

        // Make sure every spot (or its Glow) has SpotInteractable with the right index & refs
        for (int i = 0; i < spots.Count; i++)
        {
            // You can put SpotInteractable either on the spot transform or its Glow child.
            var inter = spots[i].GetComponentInChildren<SpotInteractable>(includeInactive: true);
            if (inter == null)
            {
                // fallback: put it on the spot itself
                inter = spots[i].gameObject.AddComponent<SpotInteractable>();
            }

            inter.spotIndex = i;
            inter.p1ControllerPrefab = p1ControllerPrefab;
            inter.p2ControllerPrefab = p2ControllerPrefab;

            // if you want to be extra-safe, force the glow on at start:
            if (inter.glowRoot && !inter.glowRoot.activeSelf)
                inter.glowRoot.SetActive(true);
        }
    }
}
