using UnityEngine;

public class BossRoomMusicStopper : MonoBehaviour
{
    public LevelMusicManager musicManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (musicManager)
            musicManager.StopMusic();
    }
}
