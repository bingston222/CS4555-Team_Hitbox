using UnityEngine;
using System.Collections;

public class TimeSlice : MonoBehaviour
{
    public float duration = 5f;
    public float cooldown = 10f;
    public float slowMultiplier = 0.5f;

    private bool available = true;

    public void TryActivate()
    {
        if (available)
            StartCoroutine(ActivateSlice());
    }

    IEnumerator ActivateSlice()
    {
        available = false;

        PlayerStatus player = FindObjectOfType<PlayerStatus>();
        if (player != null)
            player.moveMultiplier = slowMultiplier;

        yield return new WaitForSeconds(duration);

        if (player != null)
            player.moveMultiplier = 1f;

        yield return new WaitForSeconds(cooldown);
        available = true;
    }
}
