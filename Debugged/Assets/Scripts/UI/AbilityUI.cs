using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityUI : MonoBehaviour
{
    [Header("Basic Attack UI")]
    public Image baseIcon;                 // e.g. Player1_Ability/P1_Base_Icon/Image
    public Image baseCooldownMask;         // e.g. Player1_Ability/P1_Base_Icon/CooldownBASE
    public GameObject baseHighlight;       // optional ring/glow

    [Header("Ability UI")]
    public Image abilityIcon;              // e.g. Player1_Ability/P1_Ability_Icon/Image
    public Image abilityCooldownMask;      // e.g. Player1_Ability/P1_Ability_Icon/CooldownABILITY
    public GameObject abilityHighlight;    // optional ring/glow

    [Header("Ultimate UI")]
    public Image ultimateIcon;             // optional, if you want to tint it too
    public Image ultimateFillMask;         // e.g. Player1_Ability/P1_Ultimate_Icon/CooldownUltimate
    public GameObject ultimateGlow;        // lights up when full

    [Header("Colors")]
    public Color readyColor      = Color.white;                 // full vivid
    public Color coolingColor    = new Color(1f,1f,1f,0.35f);   // pale/faded
    public Color usingColor      = new Color(0.6f,0.9f,1f,1f);  // quick pulse color when pressed

    void Start()
    {
        // initial states
        SetIconReady(baseIcon);
        SetIconReady(abilityIcon);
        if (baseCooldownMask)    baseCooldownMask.fillAmount    = 0f;
        if (abilityCooldownMask) abilityCooldownMask.fillAmount = 0f;

        if (ultimateGlow) ultimateGlow.SetActive(false);
        if (ultimateFillMask) ultimateFillMask.fillAmount = 0f;
        if (baseHighlight) baseHighlight.SetActive(false);
        if (abilityHighlight) abilityHighlight.SetActive(false);
    }

    // ---------- Basic Attack ----------
    // percent: 1 = cooling, 0 = ready
    public void UpdateBaseCooldown(float percent)
    {
        percent = Mathf.Clamp01(percent);
        if (baseCooldownMask) baseCooldownMask.fillAmount = percent;
        if (baseIcon) baseIcon.color = (percent <= 0f) ? readyColor : coolingColor;
    }

    public void PulseBaseUsed(float duration = 0.15f)
    {
        if (!baseIcon) return;
        StopCoroutine(nameof(PulseIconRoutine));
        StartCoroutine(PulseIconRoutine(baseIcon, baseHighlight, duration));
    }

    // ---------- Ability ----------
    // percent: 1 = cooling, 0 = ready
    public void UpdateAbilityCooldown(float percent)
    {
        percent = Mathf.Clamp01(percent);
        if (abilityCooldownMask) abilityCooldownMask.fillAmount = percent;
        if (abilityIcon) abilityIcon.color = (percent <= 0f) ? readyColor : coolingColor;
    }

    public void PulseAbilityUsed(float duration = 0.15f)
    {
        if (!abilityIcon) return;
        StopCoroutine(nameof(PulseIconRoutine));
        StartCoroutine(PulseIconRoutine(abilityIcon, abilityHighlight, duration));
    }

    // ---------- Ultimate ----------
    // percent: 0..1 (fill amount)
    public void UpdateUltimateFill(float percent)
    {
        float p = Mathf.Clamp01(percent);
        if (ultimateFillMask) ultimateFillMask.fillAmount = p;
        if (ultimateGlow)     ultimateGlow.SetActive(p >= 1f);
        if (ultimateIcon)     ultimateIcon.color = (p >= 1f) ? readyColor : coolingColor;
    }

    // ---------- Helpers ----------
    void SetIconReady(Image img)
    {
        if (img) img.color = readyColor;
    }

    IEnumerator PulseIconRoutine(Image icon, GameObject highlight, float duration)
    {
        // brief color pulse + optional ring flash to show “used now”
        Color original = icon.color;
        icon.color = usingColor;
        if (highlight) highlight.SetActive(true);
        yield return new WaitForSeconds(duration);
        // Don’t force ready here; leave it to the cooldown updater to pick correct color
        if (highlight) highlight.SetActive(false);
        // If the icon is already faded by cooldown, keep it; otherwise restore vivid
        // (the Update...Cooldown will run next frame anyway)
    }
}
