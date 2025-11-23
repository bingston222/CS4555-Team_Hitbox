using UnityEngine;
using System.Collections.Generic;

public class RoomPurifier : MonoBehaviour
{
    [Header("Flip these when room purifies")]
    public ArenaStateManager arenaState;
    public BarrierRingPulse barrierRing;
    public DataParticlesColorSwap particles;
    public ArenaGridColor grid;
    public ArenaMusicManager music;

    [Header("Optional blockers")]
    public List<Collider> blockers;
    public List<UnityEngine.AI.NavMeshObstacle> obstacles;

    [Header("Enemy detection")]
    [Tooltip("If empty, auto-scan children of this object on Start.")]
    public List<EnemyDeads> explicitEnemies = new List<EnemyDeads>();

    [Tooltip("Only count this layer (-1 = any).")]
    public int enemyLayer = -1;

    [Tooltip("Include inactive children on first scan.")]
    public bool includeInactiveOnScan = true;

    private readonly HashSet<EnemyDeads> alive = new HashSet<EnemyDeads>();
    private bool done;

    void Start()
    {
        // Force boss state on entry
        music?.SetPurified(false);
        arenaState?.SetPurified(false);
        barrierRing?.SetPurified(false);
        particles?.SetPurified(false);
        grid?.SetPurified(false);

        var found = new List<EnemyDeads>();
        if (explicitEnemies != null && explicitEnemies.Count > 0)
            found.AddRange(explicitEnemies);
        else
            found.AddRange(GetComponentsInChildren<EnemyDeads>(includeInactiveOnScan));

        foreach (var e in found)
        {
            if (!e) continue;
            if (enemyLayer >= 0 && e.gameObject.layer != enemyLayer) continue;
            if (alive.Add(e)) e.OnDied += HandleEnemyDied;
        }

        Debug.Log($"[RoomPurifier:{name}] Tracking {alive.Count} enemies.");
        if (alive.Count == 0) Purify();
    }

    public void RegisterEnemy(EnemyDeads e)
    {
        if (done || !e) return;
        if (enemyLayer >= 0 && e.gameObject.layer != enemyLayer) return;
        if (alive.Add(e))
        {
            e.OnDied += HandleEnemyDied;
            Debug.Log($"[RoomPurifier:{name}] Enemy ADDED → total {alive.Count} ({e.name})");
        }
    }

    void HandleEnemyDied()
    {
        alive.RemoveWhere(x => x == null); // clean up destroyed refs
        Debug.Log($"[RoomPurifier:{name}] Enemy died → remaining {alive.Count - 1}");
        // we subtract 1 because the dying enemy is still in the set until RemoveWhere runs next frame
        if (alive.Count <= 1) Purify(); // last one just died
        else alive.RemoveWhere(x => x == null);
    }

    void Purify()
    {
        if (done) return;
        done = true;

        Debug.Log($"[RoomPurifier:{name}] ALL DEAD → PURIFY");

        arenaState?.SetPurified(true);
        barrierRing?.SetPurified(true);
        particles?.SetPurified(true);
        grid?.SetPurified(true);
        music?.SetPurified(true);

        foreach (var c in blockers) if (c) c.enabled = false;
        foreach (var o in obstacles) if (o) { o.carving = false; o.enabled = false; }

        foreach (var e in alive) if (e) e.OnDied -= HandleEnemyDied;
        alive.Clear();
        enabled = false;
    }
}
