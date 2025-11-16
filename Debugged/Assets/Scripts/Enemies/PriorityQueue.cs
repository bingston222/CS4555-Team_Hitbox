using UnityEngine;
using System.Collections;

public class PriorityQueue : MonoBehaviour
{
    [Header("Effect")]
    public float duration = 6f;

    [Header("Cooldown")]
    public float cooldown = 10f;

    private bool canCast = true;

    // Static focus target all enemies can read
    public static Transform FocusTarget { get; private set; }

    public void TryCast()
    {
        if (canCast)
            StartCoroutine(Cast());
    }

    IEnumerator Cast()
    {
        canCast = false;

        // Choose player as target
        PlayerStatus player = FindObjectOfType<PlayerStatus>();
        if (player != null)
        {
            FocusTarget = player.transform;
        }

        // Enemies with EnemyFocusReceiver will now chase FocusTarget
        yield return new WaitForSeconds(duration);

        // Clear focus
        FocusTarget = null;

        yield return new WaitForSeconds(cooldown);
        canCast = true;
    }
}
