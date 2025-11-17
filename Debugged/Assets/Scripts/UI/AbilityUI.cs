using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [Header("Basic Attack UI")]
    public Image baseCooldownMask;

    [Header("Ability UI")]
    public Image abilityCooldownMask;

    [Header("Ultimate UI")]
    public Image ultimateFillMask;
    public GameObject ultimateGlow;

    void Start()
    {
        if (ultimateGlow != null)
            ultimateGlow.SetActive(false);
    }

    public void UpdateBaseCooldown(float percent)
    {
        baseCooldownMask.fillAmount = percent; // 1 = cooling down, 0 = ready
    }

    public void UpdateAbilityCooldown(float percent)
    {
        abilityCooldownMask.fillAmount = percent;
    }

    public void UpdateUltimateFill(float percent)
    {
        ultimateFillMask.fillAmount = percent;

        if (ultimateGlow != null)
            ultimateGlow.SetActive(percent >= 1f);
    }
}
