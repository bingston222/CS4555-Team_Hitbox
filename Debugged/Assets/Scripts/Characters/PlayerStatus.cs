using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour
{
    // -------------------------
    // MOVEMENT EFFECTS
    // -------------------------
    public bool reverseInput = false;
    public float moveMultiplier = 1f;
    public bool abilitiesDisabled = false;

    // -------------------------
    // ABILITY STATUS FLAGS
    // -------------------------

    // Freeze Ability uses this
    public bool invulnerable = false;

    // HitBox Ultimate uses this
    public bool guaranteedHit = false;

    // PatchNotes Ultimate uses this
    public bool turnEnemiesFriendly = false;

    // -------------------------
    // EFFECT FUNCTIONS
    // -------------------------

    // Lag / Slow Effect
    public void ApplyLag(float duration)
    {
        StartCoroutine(LagRoutine(duration));
    }

    IEnumerator LagRoutine(float time)
    {
        moveMultiplier = 0.5f;
        yield return new WaitForSeconds(time);
        moveMultiplier = 1f;
    }

    // Reverse Controls
    public void ReverseControls(float duration)
    {
        StartCoroutine(ReverseRoutine(duration));
    }

    IEnumerator ReverseRoutine(float time)
    {
        reverseInput = true;
        yield return new WaitForSeconds(time);
        reverseInput = false;
    }

    // Silence / Disable Abilities
    public void DisableAbilities(float duration)
    {
        StartCoroutine(DisableAbilitiesRoutine(duration));
    }

    IEnumerator DisableAbilitiesRoutine(float time)
    {
        abilitiesDisabled = true;
        yield return new WaitForSeconds(time);
        abilitiesDisabled = false;
    }
}
