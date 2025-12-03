using UnityEngine;

public class FuseBoxDialogueTrigger : MonoBehaviour
{
    [Header("What we’re watching")]
    public InteractableFixable objective;   // your LevelObjectGoal

    [Header("Who plays the dialogue")]
    public SceneBeats sceneBeats;           // your SceneBeatsManager

    private bool hasPlayed = false;

    void Reset()
    {
        // auto-fill if this script is on the same object as InteractableFixable
        if (objective == null)
            objective = GetComponent<InteractableFixable>();
    }

    void Update()
    {
        if (hasPlayed) return;
        if (objective == null || sceneBeats == null) return;

        // when the fuse box is fixed for the first time…
        if (objective.isFixed)
        {
            hasPlayed = true;

            // this calls your wrapper which already uses PlayOnce
            sceneBeats.PlayZoneC_Complete();
        }
    }
}