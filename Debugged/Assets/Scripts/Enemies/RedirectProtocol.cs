using UnityEngine;
using System.Collections;

public class RedirectProtocol : MonoBehaviour
{
    public float duration = 3f;
    public float cooldown = 8f;
    private bool canCast = true;

    public void TryCast()
    {
        if (canCast)
            StartCoroutine(Cast());
    }

    IEnumerator Cast()
    {
        canCast = false;

        PlayerStatus p = FindObjectOfType<PlayerStatus>();
        if (p) p.ReverseControlsAbility(duration);

        yield return new WaitForSeconds(cooldown);
        canCast = true;
    }
}
