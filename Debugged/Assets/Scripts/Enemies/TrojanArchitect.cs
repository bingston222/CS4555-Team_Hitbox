using UnityEngine;

public class TrojanArchitect : BossBase
{
    public MineDropper mineDropper;
    public RedirectProtocol redirect;
    public Defragment defragment;

    private void Update()
    {
        if (mineDropper) mineDropper.DropIfReady();
        if (redirect) redirect.TryCast();

        // Boss becomes weakened at 50% HP → Defragment starts healing
        if (isWeakened && defragment)
            defragment.TryHeal();
    }
}
