using UnityEngine;

public class ZoneCTrigger : MonoBehaviour
{
    [Tooltip("Reference to the SceneBeats object in the scene")]
    public SceneBeats sceneBeats;

    [Tooltip("Only play once when the player first enters")]
    public bool playOnce = true;

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (playOnce && hasPlayed) return;
        if (sceneBeats == null) return;

        hasPlayed = true;

        // You already defined this in SceneBeats:
        sceneBeats.PlayZoneC_Intro();
    }

    // OPTIONAL: if you want something when leaving the zone, uncomment:
    /*
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (sceneBeats == null) return;

        sceneBeats.PlayZoneC_Complete();
    }
    */
}
