using UnityEngine;

public class Level3EndingTrigger : MonoBehaviour
{
    [Header("Assign All Fixable Skill Objects")]
    public InteractableFixable[] fixables;

    [Header("Assign All Enemies (Glitches)")]
    public GameObject[] glitches;

    [Header("Ending Dialogue Canvas (with DialogueController)")]
    public GameObject endingDialogueCanvas;

    private bool triggered = false;

    void Update()
    {
        if (triggered) return;

        if (AllFixablesCompleted() && AllGlitchesDefeated())
        {
            TriggerEndingDialogue();
        }
    }

    bool AllFixablesCompleted()
    {
        foreach (var f in fixables)
        {
            if (f == null) continue;

            // THIS IS THE REAL VARIABLE from InteractableFixable.cs
            if (!f.isFixed)   
                return false;
        }
        return true;
    }

    bool AllGlitchesDefeated()
    {
        foreach (var g in glitches)
        {
            if (g != null)   // enemy still alive
                return false;
        }
        return true;
    }

    void TriggerEndingDialogue()
    {
        triggered = true;

        if (endingDialogueCanvas != null)
            endingDialogueCanvas.SetActive(true);

        Debug.Log("LEVEL 3 COMPLETE → Triggering ending cutscene dialogue!");
    }
}
