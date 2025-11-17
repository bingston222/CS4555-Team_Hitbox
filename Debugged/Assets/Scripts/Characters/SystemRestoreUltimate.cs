using System.Collections;
using UnityEngine;

public class SystemRestoreUltimate : MonoBehaviour
{
    public KeyCode key = KeyCode.R;
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

        // Correct variable name
        var status = GetComponent<PlayerStatus>();
        status.turnEnemiesFriendly = true;

        yield return new WaitForSeconds(duration);

        status.turnEnemiesFriendly = false;

        GetComponent<UltimateCharge>().ResetCharge();

        usingUlt = false;
    }
}
