using UnityEngine;

[DisallowMultipleComponent]
public class UltimateCharge : MonoBehaviour
{
    [Header("Charge")]
    public float maxCharge = 100f;
    public float current = 0f;
    public float gainPerDamage = 1f;
    public float passivePerSecond = 0f;

    [Header("UI")]
    public AbilityUI ui;

    public bool IsFull => current >= maxCharge;

    void Awake()
    {
        if (!ui) ui = GetComponent<AbilityUI>();
        UpdateUI();
    }

    void Update()
    {
        if (passivePerSecond > 0f && !IsFull)
            AddCharge(passivePerSecond * Time.deltaTime);
    }

    public void OnDealtDamage(float dmg)
    {
        if (dmg <= 0f) return;
        AddCharge(dmg * gainPerDamage);
    }

    public void AddCharge(float amt)
    {
        current = Mathf.Clamp(current + amt, 0f, maxCharge);
        UpdateUI();
    }

    public void ResetCharge()
    {
        current = 0f;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (ui) ui.UpdateUltimateFill(Mathf.Clamp01(current / maxCharge));
    }
}
