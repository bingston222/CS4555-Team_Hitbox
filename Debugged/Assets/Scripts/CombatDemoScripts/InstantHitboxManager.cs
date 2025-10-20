using UnityEngine;

public static class InstantHitboxManager
{
    public static void Trigger(float duration)
    {
        InstantHitboxState.Activate(duration);

        var enemies = Object.FindObjectsOfType<EnemyHealth>();
        foreach (var e in enemies)
            e.ExpandHitbox(duration);
    }
}
