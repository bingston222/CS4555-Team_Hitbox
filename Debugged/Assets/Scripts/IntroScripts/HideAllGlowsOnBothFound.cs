using System.Collections;
using UnityEngine;
// (optional, for VFX Graph) using UnityEngine.VFX;

public class HideAllGlowsOnBothFound : MonoBehaviour
{
    void OnEnable()
    {
        if (GameState.I != null) GameState.I.OnBothFound += HideAll;
        else StartCoroutine(WaitAndHook());
    }

    void OnDisable()
    {
        if (GameState.I != null) GameState.I.OnBothFound -= HideAll;
    }

    IEnumerator WaitAndHook()
    {
        while (GameState.I == null) yield return null;
        GameState.I.OnBothFound += HideAll;
    }

    void HideAll(int lastFinder)
    {
        // 1) Hide glows
        foreach (var inter in GetComponentsInChildren<SpotInteractable>(true))
            if (inter && inter.glowRoot) inter.glowRoot.SetActive(false);

        // 2) Disable spots so they can't be detected / interacted with
        foreach (var inter in GetComponentsInChildren<SpotInteractable>(true))
        {
            if (!inter) continue;
            var col = inter.GetComponent<Collider>();
            if (col) col.enabled = false;
            inter.enabled = false; // safety
        }

        // 3) 🔥 Stop & clear ANY particle systems under HidingSpots
        var systems = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in systems)
        {
            if (!ps) continue;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var r = ps.GetComponent<Renderer>();
            if (r) r.enabled = false;          // hide if the system had remaining trails
            ps.gameObject.SetActive(false);     // belt & suspenders
        }

        // 3b) (Optional) If you used VFX Graph instead of ParticleSystem:
        /*
        var vfx = GetComponentsInChildren<VisualEffect>(true);
        foreach (var v in vfx)
        {
            if (!v) continue;
            v.Stop();
            v.gameObject.SetActive(false);
        }
        */
    }
}
