using UnityEngine;
using System;

public class EnemyDeads : MonoBehaviour
{
    public event Action OnDied;
    bool fired;

    public void NotifyDied()
    {
        if (fired) return;
        fired = true;
        OnDied?.Invoke();
    }

    // Fallback if you DESTROY the enemy and forgot to call NotifyDied():
    void OnDestroy()
    {
        if (!fired) OnDied?.Invoke();
    }}
