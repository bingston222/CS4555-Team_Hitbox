
using UnityEngine;

public class TestBossDeathTrigger : MonoBehaviour
{
    void Update()
    {
        // Press P to simulate the boss dying
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("🟦 Simulating Boss Death");
            FindObjectOfType<ArenaStateManager>()?.SetPurified(true);
            FindObjectOfType<BarrierRingPulse>()?.SetPurified(true);
            FindObjectOfType<DataParticlesColorSwap>()?.SetPurified(true);
            FindObjectOfType<ArenaGridColor>()?.SetPurified(true);
            FindObjectOfType<ArenaMusicManager>()?.SetPurified(true);
        }
    }
}
