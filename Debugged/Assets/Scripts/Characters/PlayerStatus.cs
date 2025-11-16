using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour
{
    public bool reverseInput = false;
    public float moveMultiplier = 1f;   // 1 = normal, <1 slow, >1 fast
    public bool abilitiesDisabled = false;

    public void ApplyLag(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(LagRoutine(duration));
    }

    IEnumerator LagRoutine(float time)
    {
        moveMultiplier = 0.5f;
        yield return new WaitForSeconds(time);
        moveMultiplier = 1f;
    }

    public void ReverseControls(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ReverseRoutine(duration));
    }

    IEnumerator ReverseRoutine(float time)
    {
        reverseInput = true;
        yield return new WaitForSeconds(time);
        reverseInput = false;
    }

    public void DisableAbilities(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(DisableAbilitiesRoutine(duration));
    }

    IEnumerator DisableAbilitiesRoutine(float time)
    {
        abilitiesDisabled = true;
        yield return new WaitForSeconds(time);
        abilitiesDisabled = false;
    }
}
