using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour
{
    // ---- INVULNERABILITY ----
    private bool invulnerable = false;
    public bool IsInvulnerable => invulnerable;

    public void SetInvulnerable(bool state)
    {
        invulnerable = state;
    }

    // ---- GUARANTEED HIT (InstantHitboxUltimate) ----
    private bool guaranteedHit = false;
    public bool IsGuaranteedHit => guaranteedHit;

    public void GuaranteedHitAbility(float duration)
    {
        StartCoroutine(GuaranteedHitRoutine(duration));
    }

    IEnumerator GuaranteedHitRoutine(float duration)
    {
        guaranteedHit = true;
        yield return new WaitForSeconds(duration);
        guaranteedHit = false;
    }

    // ---- TURN ENEMIES FRIENDLY (SystemRestoreUltimate) ----
    private bool enemiesFriendly = false;
    public bool AreEnemiesFriendly => enemiesFriendly;

    public void SetEnemiesFriendly(bool state)
    {
        enemiesFriendly = state;
    }

    public void EnemiesFriendlyAbility(float duration)
    {
        StartCoroutine(FriendlyRoutine(duration));
    }

    IEnumerator FriendlyRoutine(float duration)
    {
        enemiesFriendly = true;
        yield return new WaitForSeconds(duration);
        enemiesFriendly = false;
    }

    // ---- REVERSE CONTROLS (RedirectProtocol) ----
    private bool reverseControls = false;
    public bool ReverseControls => reverseControls;

    public void ReverseControlsAbility(float duration)
    {
        StartCoroutine(ReverseRoutine(duration));
    }

    IEnumerator ReverseRoutine(float duration)
    {
        reverseControls = true;
        yield return new WaitForSeconds(duration);
        reverseControls = false;
    }

    // ---- LAG / SLOW EFFECT (PulseBeam) ----
    private float lagMultiplier = 1f;
    public float LagMultiplier => lagMultiplier;

    public void ApplyLag(float multiplier)
    {
        lagMultiplier = multiplier;
    }

    // ---- MOVEMENT MULTIPLIER (TimeSlice) ----
    public float moveMultiplier = 1f;

    // ---- ABILITY DISABLE (ClockReset) ----
    public bool abilitiesDisabled = false;

    public void DisableAbilities(float duration)
    {
        StartCoroutine(DisableRoutine(duration));
    }

    IEnumerator DisableRoutine(float duration)
    {
        abilitiesDisabled = true;
        yield return new WaitForSeconds(duration);
        abilitiesDisabled = false;
    }
}
