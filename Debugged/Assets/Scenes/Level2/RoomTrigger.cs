using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public SceneTransition sceneTransition;  // drag your FadeCanvas here

    private bool player1Inside = false;
    private bool player2Inside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1"))
        {
            player1Inside = true;
        }
        else if (other.CompareTag("Player2"))
        {
            player2Inside = true;
        }

        // If both are inside → Fade
        if (player1Inside && player2Inside)
        {
            sceneTransition.BeginTransition();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player1"))
            player1Inside = false;

        if (other.CompareTag("Player2"))
            player2Inside = false;
    }
}
