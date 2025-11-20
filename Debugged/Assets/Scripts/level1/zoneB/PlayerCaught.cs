using UnityEngine;
using System.Collections;

public class PlayerCaught : MonoBehaviour
{
    public MonoBehaviour[] movementScripts;
    public Animator animator;
    public string caughtTrigger = "Caught";
    public float pushBackDistance = 0.4f;

    public void OnCaught(float freezeSeconds, Vector3 cameraPos)
    {
        StopAllCoroutines();
        StartCoroutine(FreezeRoutine(freezeSeconds, cameraPos));
    }

    IEnumerator FreezeRoutine(float freezeSeconds, Vector3 cameraPos)
    {
        Vector3 dir = (transform.position - cameraPos);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            transform.position += dir.normalized * pushBackDistance;

        foreach (var s in movementScripts) if (s) s.enabled = false;
        if (animator && !string.IsNullOrEmpty(caughtTrigger)) animator.SetTrigger(caughtTrigger);

        yield return new WaitForSeconds(freezeSeconds);

        foreach (var s in movementScripts) if (s) s.enabled = true;
    }
}
