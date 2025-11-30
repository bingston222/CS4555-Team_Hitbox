using System.Collections;
using UnityEngine;

public class SystemRestoreUltimate : MonoBehaviour
{
    public KeyCode key = KeyCode.RightShift;
    public float duration = 10f;

    bool usingUlt = false;

    void Update()
    {
        var charge = GetComponent<UltimateCharge>();

        if (Input.GetKeyDown(key) && charge.IsFull && !usingUlt)
            StartCoroutine(DoUltimate());
    }

    IEnumerator DoUltimate()
    {
        usingUlt = true;

        var status = GetComponent<PlayerStatus>();
        status.EnemiesFriendlyAbility(duration);

        yield return new WaitForSeconds(duration);

        GetComponent<UltimateCharge>().ResetCharge();
        usingUlt = false;
    }
}
