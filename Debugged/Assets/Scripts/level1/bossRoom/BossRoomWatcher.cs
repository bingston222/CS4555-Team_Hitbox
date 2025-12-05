using UnityEngine;
using System.Collections;

public class BossRoomWatcher : MonoBehaviour
{
    [Header("Bosses in this room")]
    public GameObject[] bosses;

    [Header("Dialogue")]
    public SceneBeats sceneBeats;

    [Header("Scene Transition")]
    public SceneTransition sceneTransition;

    private bool hasPlayed = false;

    void Update()
    {
        if (hasPlayed) return;
        if (sceneBeats == null || bosses == null || bosses.Length == 0) return;

        bool allDead = true;

        foreach (var boss in bosses)
        {
            if (boss != null)
            {
                allDead = false;
                break;
            }
        }

        if (!allDead) return;

        hasPlayed = true;
        StartCoroutine(BossRoomSequence());
    }

    private IEnumerator BossRoomSequence()
{
    // 1. Play final dialogue
    yield return sceneBeats.PlayOnce("bossRoom.done", sceneBeats.bossRoom_Complete);

    // 2. WAIT before fading
    yield return new WaitForSeconds(8f);   // <--- Wait 8 seconds

    // 3. Fade out
    if (sceneTransition != null)
        sceneTransition.BeginTransition();
}
}