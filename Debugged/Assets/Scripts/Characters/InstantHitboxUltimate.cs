using System.Collections;
using UnityEngine;

public class InstantHitboxUltimate : MonoBehaviour
{
    public KeyCode key = KeyCode.LeftShift;   // LEFT SHIFT ✔
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

        GetComponent<PlayerStatus>().guaranteedHit = true;

        yield return new WaitForSeconds(duration);

        GetComponent<PlayerStatus>().guaranteedHit = false;
        GetComponent<UltimateCharge>().ResetCharge();

        usingUlt = false;
    }
}
