using UnityEngine;

// simplest: music only on entry (no visuals)
public class MusicTriggerZone : MonoBehaviour
{
    public ArenaMusicManager musicManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            musicManager.SetPurified(false); // <- boss state on entry
    }
}
