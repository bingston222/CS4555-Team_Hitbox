using UnityEngine;

public class CombatTestObjective : MonoBehaviour
{
    public string objectiveId = "ZoneA_Orbs";
    public int totalOrbs = 2; // number of orbs that must be destroyed
    private int destroyed = 0;

    public static System.Action<string> OnOrbDestroyedStatic;

    void OnEnable()  => OnOrbDestroyedStatic += OnOrbDestroyed;
    void OnDisable() => OnOrbDestroyedStatic -= OnOrbDestroyed;

    private void OnOrbDestroyed(string id)
{
    if (id != objectiveId) return;
    destroyed++;

    // ---- FIRST ORB DESTROYED ----
    if (destroyed == 1)
    {
        if (Adialogue.Instance)
        {
            Adialogue.Instance.StartCoroutine(
                Adialogue.Instance.ShowSubtitle("Good hit—one more!", 1.4f, 0.18f)
            );
        }
    }

    // ---- BOTH ORBS DESTROYED ----
    if (destroyed >= totalOrbs)
    {
        Debug.Log("✅ Both test orbs destroyed.");

        // HQ: final calibration line
        if (Adialogue.Instance)
        {
            Adialogue.Instance.StartCoroutine(
                Adialogue.Instance.ShowSubtitle("Calibration complete. Nice work! Opening the firewall door...", 2.2f, 0.18f)
            );
        }

        // open door in steps
        IntroController intro = FindObjectOfType<IntroController>();
        if (intro)
        {
            intro.StartCoroutine(intro.OpenDoorInSteps());
        }
    }
}


    // Called by TestOrb when destroyed
    public static void OrbDestroyed(string id)
    {
        OnOrbDestroyedStatic?.Invoke(id);
    }
}
