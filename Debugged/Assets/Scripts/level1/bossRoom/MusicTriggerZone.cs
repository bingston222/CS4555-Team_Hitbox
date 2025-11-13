using UnityEngine;

public class MusicTriggerZone : MonoBehaviour
{
    public ArenaMusicManager musicManager;
    public bool purifiedRoom = false; // Set this true if this room uses purified music

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // make sure your player has the tag "Player"
        {
            musicManager.SetPurified(purifiedRoom);
        }
    }
}