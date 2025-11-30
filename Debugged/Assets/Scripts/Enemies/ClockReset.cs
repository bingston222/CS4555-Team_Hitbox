using UnityEngine;
using System.Collections;

public class ClockReset : MonoBehaviour
{
    public float duration = 3f;
    public float cooldown = 12f;

    private bool canCast = true;

    public void TryCast()
    {
        if (canCast)
            StartCoroutine(Cast());
    }

    IEnumerator Cast()
    {
        canCast = false;

        PlayerStatus ps = FindObjectOfType<PlayerStatus>();
        if (ps != null)
            ps.DisableAbilities(duration);

        yield return new WaitForSeconds(cooldown);
        canCast = true;
    }
}
