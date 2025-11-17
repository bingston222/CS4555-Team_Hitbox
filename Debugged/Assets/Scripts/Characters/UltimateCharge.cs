using UnityEngine;

public class UltimateCharge : MonoBehaviour
{
    public float maxCharge = 100f;
    public float currentCharge = 0f;

    AbilityUI ui;

    public bool IsFull => currentCharge >= maxCharge;

    void Start()
    {
        ui = GetComponent<AbilityUI>();
        ui.UpdateUltimateFill(0f);
    }

    public void AddCharge(float amount)
    {
        currentCharge += amount;
        currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);

        float percent = currentCharge / maxCharge;
        ui.UpdateUltimateFill(percent);
    }

    public void ResetCharge()
    {
        currentCharge = 0f;
        ui.UpdateUltimateFill(0f);
    }
}
