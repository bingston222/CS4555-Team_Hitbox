using UnityEngine;
using System.Collections;

public static class InstantHitboxState
{
    public static bool IsActive { get; private set; }
    static float endTime;

    public static void Activate(float duration)
    {
        IsActive = true;
        endTime = Time.time + duration;
    }

    // Call each frame (from your controller) to auto-expire.
    public static void Tick()
    {
        if (IsActive && Time.time >= endTime)
            IsActive = false;
    }
}
