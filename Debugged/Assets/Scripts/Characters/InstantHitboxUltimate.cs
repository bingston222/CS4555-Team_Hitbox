using UnityEngine;
using System.Collections;

public class InstantHitboxUltimate : MonoBehaviour
{
    public KeyCode key = KeyCode.LeftShift;
    public float duration = 8f;
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

        // Activate guaranteed hit for the duration
        GetComponent<PlayerStatus>().GuaranteedHitAbility(duration);

        yield return new WaitForSeconds(duration);

        // Reset ultimate charge after use
        GetComponent<UltimateCharge>().ResetCharge();
        usingUlt = false;
    }
}
