using UnityEngine;

public class FirewallOverseer : BossBase
{
    public PulseBeam pulseBeam;
    public FirewallShield shield;
    public Overheat overheat;

    protected override void OnWeakened()
    {
        shield.ActivateShield();
    }

    private void Update()
    {
        if (!pulseBeam.isFiring)
            pulseBeam.TryFire();

        if (currentHP <= maxHP * 0.5f && !overheat.hasTriggered)
            overheat.TriggerOverheat();
    }
}
