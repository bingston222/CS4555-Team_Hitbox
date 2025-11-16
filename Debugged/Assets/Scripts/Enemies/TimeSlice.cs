using UnityEngine;
using System.Collections;

public class TimeSlice : MonoBehaviour
{
    [Header("Timing")]
    public float duration = 5f;
    public float cooldown = 10f;

    [Header("Player Speed Multipliers")]
    public float slowMultiplier = 0.5f;  // player moves slower in lag zone

    private bool available = true;

    public void TryActivate()
    {
        if (available)
            StartCoroutine(ActivateSlice());
    }

    IEnumerator ActivateSlice()
    {
        available = false;

        // Find player status (assumes one player in scene)
        PlayerStatus player = FindObjectOfType<PlayerStatus>();
        if (player != null)
        {
            // Apply slow
            player.moveMultiplier = slowMultiplier;
        }

        yield return new WaitForSeconds(duration);

        // Reset back to normal speed
        if (player != null)
        {
            player.moveMultiplier = 1f;
        }

        // Wait for cooldown before next cast
        yield return new WaitForSeconds(cooldown);
        available = true;
    }
}
