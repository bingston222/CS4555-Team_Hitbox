using UnityEngine;

public class SceneBeats : MonoBehaviour
{
    [Header("Dialogue UI")]
    public UnifiedDialogueController dialogue;

    [Header("Icons/SFX (optional defaults)")]
    public Sprite radioIcon;
    public AudioClip radioPing;

    [Header("Dialogue Sets (fill in Inspector)")]
    public UnifiedDialogueController.DialogueLine[] calibrationIntro;   // “Let’s run system calibration…”
    public UnifiedDialogueController.DialogueLine[] calibrationGuide;   // “Move / Run / Jump” keys
    public UnifiedDialogueController.DialogueLine[] calibrationDone;    // “Calibration complete.”
    public UnifiedDialogueController.DialogueLine[] doorIntro;          // “Stand on your panels…”
    public UnifiedDialogueController.DialogueLine[] doorOpen;           // “Door unlocked.”

    // Utility: play any set and wait (optionally lock input outside this script)
    public System.Collections.IEnumerator Play(UnifiedDialogueController.DialogueLine[] set)
    {
        bool done = false;
        dialogue.OnConversationFinished.AddListener(() => done = true);
        dialogue.PlayLines(set);
        yield return new WaitUntil(() => done);
        dialogue.OnConversationFinished.RemoveAllListeners();
    }
}
