using UnityEngine;

public class BossRoomWatcher : MonoBehaviour
{
    [Header("Bosses in this room")]
    public GameObject[] bosses;      // drag each boss here

    [Header("Dialogue")]
    public SceneBeats sceneBeats;    // drag your SceneBeatsManager

    private bool hasPlayed = false;

    void Update()
    {
        if (hasPlayed) return;
        if (sceneBeats == null || bosses == null || bosses.Length == 0) return;

        bool allDead = true;

        // If any boss GameObject still exists, they’re not all dead yet
        foreach (var boss in bosses)
        {
            if (boss != null)      // still alive
            {
                allDead = false;
                break;
            }
        }

        if (!allDead) return;

        hasPlayed = true;

        // Use your SceneBeats helper (respects PlayOnce guard too)
        sceneBeats.PlayBossRoom_Complete();
    }
}
