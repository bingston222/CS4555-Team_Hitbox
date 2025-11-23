using UnityEngine;

public class RoomStateTrigger : MonoBehaviour
{
    [Header("Set the state when the Player ENTERS this volume")]
    public bool purifiedOnEnter = false; // false = corrupted/boss, true = purified/victory

    [Header("Targets to flip")]
    public ArenaMusicManager music;
    public ArenaStateManager arenaState;
    public BarrierRingPulse barrierRing;
    public DataParticlesColorSwap particles;
    public ArenaGridColor grid;

    [Header("Optional blockers to disable when purified")]
    public Collider[] blockers;
    public UnityEngine.AI.NavMeshObstacle[] obstacles;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Music
        music?.SetPurified(purifiedOnEnter);

        // Visuals / lights / particles / ring
        arenaState?.SetPurified(purifiedOnEnter);
        barrierRing?.SetPurified(purifiedOnEnter);
        particles?.SetPurified(purifiedOnEnter);
        grid?.SetPurified(purifiedOnEnter);

        // If this is the “victory/purified” zone, open the gate
        if (purifiedOnEnter)
        {
            foreach (var c in blockers) if (c) c.enabled = false;
            foreach (var o in obstacles) if (o) { o.carving = false; o.enabled = false; }
        }
    }
}