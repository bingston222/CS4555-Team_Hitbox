using System.Collections;
using UnityEngine;

public class HideAllGlowsOnBothFound : MonoBehaviour
{
    void OnEnable()
    {
        // If GameState isn't ready yet, wait a frame until it is.
        if (GameState.I != null) GameState.I.OnBothFound += HideAll;
        else StartCoroutine(WaitAndHook());
    }

    void OnDisable()
    {
        if (GameState.I != null) GameState.I.OnBothFound -= HideAll;
    }

    IEnumerator WaitAndHook()
    {
        // wait until GameState singleton exists
        while (GameState.I == null) yield return null;
        GameState.I.OnBothFound += HideAll;
    }

    void HideAll(int lastFinder)
    {
        Debug.Log("[HideAllGlowsOnBothFound] Both found → hiding all glows.");

        // 1) Preferred path: use SpotInteractable.glowRoot if present
        var interactables = GetComponentsInChildren<SpotInteractable>(true);
        foreach (var inter in interactables)
        {
            if (inter && inter.glowRoot) inter.glowRoot.SetActive(false);
        }

        // 2) Fallback: turn off any child actually named "Glow"
        var trs = GetComponentsInChildren<Transform>(true);
        foreach (var t in trs)
        {
            if (t && t.name == "Glow") t.gameObject.SetActive(false);
        }
    }
}
