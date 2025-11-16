using UnityEngine;

public class KernelWarden : BossBase
{
    [Header("Abilities")]
    public TimeSlice timeSlice;
    public PriorityQueue priorityQueue;
    public ClockReset clockReset;

    void Update()
    {
        // Fire abilities on their own internal cooldowns
        if (timeSlice != null)      timeSlice.TryActivate();
        if (priorityQueue != null)  priorityQueue.TryCast();
        if (clockReset != null)     clockReset.TryCast();
    }

    // Optional: when weakened at 50% HP, we could make abilities faster/stronger
    protected override void OnWeakened()
    {
        if (timeSlice != null)     timeSlice.cooldown *= 0.7f;
        if (priorityQueue != null) priorityQueue.cooldown *= 0.7f;
        if (clockReset != null)    clockReset.cooldown *= 0.7f;
    }
}
