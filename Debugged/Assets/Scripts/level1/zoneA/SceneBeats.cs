using System.Collections.Generic;
using UnityEngine;

public class SceneBeats : MonoBehaviour
{
    [Header("Dialogue UI")]
    public UnifiedDialogueController dialogue;

    [Header("Dialogue Sets (fill in Inspector)")]
    public UnifiedDialogueController.DialogueLine[] calibrationIntro;
    public UnifiedDialogueController.DialogueLine[] calibrationGuide;
    public UnifiedDialogueController.DialogueLine[] calibrationDone;
    public UnifiedDialogueController.DialogueLine[] doorIntro;
    public UnifiedDialogueController.DialogueLine[] doorOpen;

    // In SceneBeats
public UnifiedDialogueController.DialogueLine[] zoneB_Intro;
public UnifiedDialogueController.DialogueLine[] zoneB_PuzzleHint;
public UnifiedDialogueController.DialogueLine[] zoneB_Complete;


    // --- NEW: one-shot + busy guards ---
    private readonly HashSet<string> played = new HashSet<string>();
    private bool running = false;

    // Basic play (respects 'running' guard)
    public System.Collections.IEnumerator Play(UnifiedDialogueController.DialogueLine[] set)
    {
        if (set == null || set.Length == 0 || dialogue == null) yield break;
        if (running) yield break;                  // <- prevents replay while another is running

        running = true;
        bool done = false;
        dialogue.OnConversationFinished.AddListener(() => done = true);
        dialogue.PlayLines(set);
        yield return new WaitUntil(() => done);
        dialogue.OnConversationFinished.RemoveAllListeners();
        running = false;
    }

    // --- NEW: one-shot wrapper ---
    public System.Collections.IEnumerator PlayOnce(string key, UnifiedDialogueController.DialogueLine[] set)
    {
        if (played.Contains(key)) yield break;     // <- already played in this scene
        played.Add(key);
        yield return Play(set);
    }

    // Call from Timeline Signal / UnityEvent / other scripts
public void PlayZoneB_Intro()       => StartCoroutine(PlayOnce("zoneB.intro", zoneB_Intro));
public void PlayZoneB_PuzzleHint()  => StartCoroutine(PlayOnce("zoneB.hint",  zoneB_PuzzleHint));
public void PlayZoneB_Complete()    => StartCoroutine(PlayOnce("zoneB.done",  zoneB_Complete));

}
